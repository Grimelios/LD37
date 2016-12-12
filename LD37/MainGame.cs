using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using LD37.Entities;
using LD37.Entities.Abstract;
using LD37.Entities.Organization;
using LD37.Entities.Platforms;
using LD37.Input;
using LD37.Json;
using LD37.Levels;
using LD37.Messaging;
using LD37.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Ninject;

namespace LD37
{
	using EntityMap = Dictionary<string, List<Entity>>;

	internal enum OriginLocations
	{
		Center,
		Default
	}

	internal class MainGame : Game
	{
		private const int DefaultScreenWidth = 1024;
		private const int DefaultScreenHeight = 768;
		private const int Gravity = 30;

		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;

		private Camera camera;
		private InputGenerator inputGenerator;
		private PhysicsDebugDrawer physicsDebugDrawer;
		private Scene scene;
		private World world;

		public MainGame()
		{
			graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = DefaultScreenWidth,
				PreferredBackBufferHeight = DefaultScreenHeight
			};

			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			Window.Title = "Ludum Dare 37";
		}
		
		protected override void Initialize()
		{
			world = new World(new Vector2(0, Gravity));
			scene = new Scene();

			IKernel kernel = new StandardKernel();
			kernel.Bind<ContentLoader>().ToConstant(new ContentLoader(Content));
			kernel.Bind<InteractionSystem>().ToSelf().InSingletonScope();
			kernel.Bind<MessageSystem>().ToSelf().InSingletonScope();
			kernel.Bind<PhysicsFactory>().ToSelf().InSingletonScope();
			kernel.Bind<PhysicsHelper>().ToSelf().InSingletonScope();
			kernel.Bind<PrimitiveDrawer>().ToSelf().InSingletonScope();
			kernel.Bind<Scene>().ToConstant(scene);
			kernel.Bind<StandardKernel>().ToConstant((StandardKernel)kernel);
			kernel.Bind<World>().ToConstant(world);

			JsonConvert.DefaultSettings = () => new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto,
				ContractResolver = new DIContractResolver(kernel)
			};

			camera = kernel.Get<Camera>();
			inputGenerator = kernel.Get<InputGenerator>();
			physicsDebugDrawer = kernel.Get<PhysicsDebugDrawer>();

			CreatePrimaryLayer(kernel);

			kernel.Get<Editor>();

			base.Initialize();
		}

		private void CreatePrimaryLayer(IKernel kernel)
		{
			Tilemap tilemap = JsonUtilities.Deserialize<Tilemap>("Tilemaps/OneRoom.json");
			Player player = kernel.Get<Player>();
			player.LoadPosition = new Vector2(600, 600);

			EntityLayer primaryLayer = new EntityLayer(new []
			{
				"Tile",
				"Player"
			}, new []
			{
				"Tilemap",
				"Tile",
				"Wire",
				"Laser",
				"Player",
			});

			EntityMap entityMap = primaryLayer.EntityMap;
			entityMap["Player"].Add(player);
			entityMap["Tilemap"].Add(tilemap);
			scene.LayerMap.Add("Primary", primaryLayer);

			AddRoomEdges(kernel, tilemap);
			CreateTiles(kernel);

			LevelSystem levelSystem = kernel.Get<LevelSystem>();
			levelSystem.Refresh(Point.Zero, false);
		}

		private void CreateTiles(IKernel kernel)
		{
			Tile[,] tiles = new Tile[Constants.RoomWidth - 2, Constants.RoomHeight - 2];
			List<Entity> tileList = new List<Entity>();

			for (int i = 0; i < Constants.RoomHeight - 2; i++)
			{
				for (int j = 0; j < Constants.RoomWidth - 2; j++)
				{
					Tile tile = kernel.Get<Tile>();
					tile.LoadPosition = new Vector2(j, i);
					tiles[j, i] = tile;
					tileList.Add(tile);
				}
			}

			EntityMap entityMap = scene.LayerMap["Primary"].EntityMap;
			entityMap["Tile"].AddRange(tileList);
		}

		private void AddRoomEdges(IKernel kernel, Tilemap tilemap)
		{
			Vector2 topLeft = Vector2.One;
			Vector2 topRight = new Vector2(Constants.RoomWidth - 1, 1);
			Vector2 bottomLeft = new Vector2(1, Constants.RoomHeight - 1);
			Vector2 bottomRight = new Vector2(Constants.RoomWidth - 1, Constants.RoomHeight - 1);

			PhysicsFactory physicsFactory = kernel.Get<PhysicsFactory>();
			Body body = physicsFactory.CreateBody(tilemap);

			physicsFactory.AttachEdge(body, topLeft, topRight, Units.Meters);
			physicsFactory.AttachEdge(body, topLeft, bottomLeft, Units.Meters);
			physicsFactory.AttachEdge(body, topRight, bottomRight, Units.Meters);
			physicsFactory.AttachEdge(body, bottomLeft, bottomRight, Units.Meters);

			world.ProcessChanges();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
		}

		protected override void UnloadContent()
		{
		}

		protected override void Update(GameTime gameTime)
		{
			float dt = (float)gameTime.ElapsedGameTime.Milliseconds / 1000;

			inputGenerator.GenerateInputMessages();
			world.Step(dt);
			scene.Update(dt);
			camera.Update(dt);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(new Color(90, 90, 90));

			spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.Transform);
			scene.Render(spriteBatch);
			physicsDebugDrawer.Render(spriteBatch);
			spriteBatch.End();
		}
	}
}
