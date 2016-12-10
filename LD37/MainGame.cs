using System;
using FarseerPhysics.Dynamics;
using LD37.Entities.Lasers;
using LD37.Entities.Organization;
using LD37.Input;
using LD37.Messaging;
using LD37.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ninject;

namespace LD37
{
	internal enum OriginLocations
	{
		Center,
		Default
	}

	internal class MainGame : Game
	{
		private const int DefaultScreenWidth = 1024;
		private const int DefaultScreenHeight = 768;

		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;

		private Camera camera;
		private InputGenerator inputGenerator;
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
			world = new World(new Vector2(0, 10));

			IKernel kernel = new StandardKernel();
			kernel.Bind<ContentLoader>().ToConstant(new ContentLoader(Content));
			kernel.Bind<MessageSystem>().ToSelf().InSingletonScope();
			kernel.Bind<PhysicsFactory>().ToConstant(new PhysicsFactory(world));
			kernel.Bind<PhysicsHelper>().ToConstant(new PhysicsHelper(world));
			kernel.Bind<PrimitiveDrawer>().ToSelf().InSingletonScope();

			camera = kernel.Get<Camera>();
			inputGenerator = kernel.Get<InputGenerator>();

			CreatePrimaryLayer(kernel);

			base.Initialize();
		}

		private void CreatePrimaryLayer(IKernel kernel)
		{
			LaserSource laserSource = kernel.Get<LaserSource>();

			EntityLayer primaryLayer = new EntityLayer(new Type[0], new []
			{
				typeof(LaserSource),
				typeof(Laser)
			});

			primaryLayer.Add(typeof(LaserSource), laserSource);
			primaryLayer.Add(typeof(Laser), laserSource.Laser);

			scene = new Scene();
			scene.LayerMap.Add("Primary", primaryLayer);
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
			GraphicsDevice.Clear(Color.Black);

			spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.Transform);
			scene.Render(spriteBatch);
			spriteBatch.End();
		}
	}
}
