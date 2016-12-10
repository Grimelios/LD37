using FarseerPhysics.Dynamics;
using LD37.Entities;
using LD37.Entities.Lasers;
using LD37.Entities.Organization;
using LD37.Input;
using LD37.Interfaces;
using LD37.Json;
using LD37.Messaging;
using LD37.Messaging.Input;
using LD37.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Ninject;

namespace LD37
{
	internal enum OriginLocations
	{
		Center,
		Default
	}

	internal class MainGame : Game, IMessageReceiver
	{
		private const int DefaultScreenWidth = 1024;
		private const int DefaultScreenHeight = 768;
		private const int RoomWidth = 32;
		private const int RoomHeight = 24;

		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;

		private Camera camera;
		private InputGenerator inputGenerator;
		private PhysicsDebugDrawer physicsDebugDrawer;
		private Scene scene;
		private World world;

		private Tile tile;

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
			world = new World(new Vector2(0, 10));

			IKernel kernel = new StandardKernel();
			kernel.Bind<ContentLoader>().ToConstant(new ContentLoader(Content));
			kernel.Bind<InteractionSystem>().ToSelf().InSingletonScope();
			kernel.Bind<MessageSystem>().ToSelf().InSingletonScope();
			kernel.Bind<PhysicsFactory>().ToSelf().InSingletonScope();
			kernel.Bind<PhysicsHelper>().ToSelf().InSingletonScope();
			kernel.Bind<PrimitiveDrawer>().ToSelf().InSingletonScope();
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

			kernel.Get<MessageSystem>().Subscribe(MessageTypes.Mouse, this);

			base.Initialize();
		}

		private void CreatePrimaryLayer(IKernel kernel)
		{
			Tilemap tilemap = JsonUtilities.Deserialize<Tilemap>("Tilemaps/OneRoom.json");
			AbstractLaserSource laserSource = kernel.Get<FixedLaserSource>();
			Player player = kernel.Get<Player>();
			player.LoadPosition = new Vector2(800, 200);

			EntityLayer primaryLayer = new EntityLayer(new []
			{
				typeof(Tile),
				typeof(Player)
			}, new []
			{
				typeof(Tile),
				typeof(Laser),
				typeof(Player),
				typeof(Tilemap)
			});

			tile = kernel.Get<Tile>();
			tile.Position = new Vector2(100);
			tile.AttachedEntity = laserSource;

			primaryLayer.Add(typeof(Tile), tile);
			primaryLayer.Add(typeof(Laser), laserSource.Laser);
			primaryLayer.Add(typeof(Player), player);
			primaryLayer.Add(typeof(Tilemap), tilemap);

			scene = new Scene();
			scene.LayerMap.Add("Primary", primaryLayer);

			AddRoomEdges(kernel, tilemap);

			laserSource.Position = new Vector2(100);
			laserSource.Rotation = 0;
			laserSource.Color = Color.Red;
		}

		private void AddRoomEdges(IKernel kernel, Tilemap tilemap)
		{
			Vector2 topLeft = Vector2.One;
			Vector2 topRight = new Vector2(RoomWidth - 1, 1);
			Vector2 bottomLeft = new Vector2(1, RoomHeight - 1);
			Vector2 bottomRight = new Vector2(RoomWidth - 1, RoomHeight - 1);

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

		public void Receive(GameMessage message)
		{
			MouseData data = ((MouseMessage)message).Data;

			if (data.LeftClickState == ClickStates.PressedThisFrame)
			{
				tile.Flip();
			}
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
			GraphicsDevice.Clear(Color.Black);

			spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.Transform);
			scene.Render(spriteBatch);
			physicsDebugDrawer.Render(spriteBatch);
			spriteBatch.End();
		}
	}
}
