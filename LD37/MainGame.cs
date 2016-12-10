using FarseerPhysics.Dynamics;
using LD37.Entities.Lasers;
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
		private World world;

		private LaserSource laserSource;

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
			kernel.Bind<PhysicsFactory>().ToSelf().InSingletonScope();
			kernel.Bind<World>().ToConstant(world);

			camera = kernel.Get<Camera>();
			inputGenerator = kernel.Get<InputGenerator>();

			laserSource = kernel.Get<LaserSource>();
			laserSource.Position = new Vector2(400);

			base.Initialize();
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
			camera.Update(dt);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.Transform);
			laserSource.Render(spriteBatch);
			spriteBatch.End();
		}
	}
}
