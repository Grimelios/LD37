using System;
using System.Collections.Generic;
using LD37.Entities;
using LD37.Entities.Abstract;
using LD37.Entities.Lasers;
using LD37.Entities.Organization;
using LD37.Entities.Platforms;
using LD37.Input;
using LD37.Interfaces;
using LD37.Messaging;
using LD37.Messaging.Input;
using LD37.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Ninject;

namespace LD37
{
	using EntityMap = Dictionary<string, List<Entity>>;

	internal class Editor : IRenderable, IMessageReceiver
	{
		private const float PiOver8 = MathHelper.PiOver4 / 2;

		private enum EditableEntityTypes
		{
			FixedLaserSource,
			LaserReceiver,
			KeyButton,
			Mirror,
			Rotator,
			SlidingDoor,
			Switch,
			Wire
		}

		private Tile[,] tiles;
		private Entity selectedEntity;
		private MessageSystem messageSystem;
		private PrimitiveDrawer primitiveDrawer;
		private StandardKernel kernel;
		private EntityMap entityMap;
		private Wire wire;

		private IPowered wireEntity;
		private EditableEntityTypes selectedEntityType;

		private bool editorEnabled;
		private bool shiftHeld;
		private bool pHeld;
		private bool platformMode;
		private bool platformInProgress;

		private Vector2 platformStart;
		private Vector2 platformEnd;
		private List<Platform> platforms;

		public Editor(MessageSystem messageSystem, PrimitiveDrawer primitiveDrawer, Scene scene, StandardKernel kernel)
		{
			this.primitiveDrawer = primitiveDrawer;
			this.messageSystem = messageSystem;
			this.kernel = kernel;

			tiles = scene.RetrieveTiles();
			entityMap = scene.LayerMap["Primary"].EntityMap;

			messageSystem.Subscribe(MessageTypes.Keyboard, this);
			messageSystem.Subscribe(MessageTypes.Mouse, this);
		}

		public List<Platform> Platforms
		{
			set { platforms = value; }
		}

		public void Receive(GameMessage message)
		{
			switch (message.Type)
			{
				case MessageTypes.Keyboard:
					HandleKeyboard(((KeyboardMessage)message).Data);
					break;

				case MessageTypes.Mouse:
					HandleMouse(((MouseMessage)message).Data);
					break;
			}
		}

		private void HandleKeyboard(KeyboardData data)
		{
			bool controlHeld = false;

			shiftHeld = false;
			pHeld = false;

			foreach (Keys key in data.KeysDown)
			{
				switch (key)
				{
					case Keys.LeftControl:
					case Keys.RightControl:
						controlHeld = true;
						break;

					case Keys.LeftShift:
					case Keys.RightShift:
						shiftHeld = true;
						break;

					case Keys.P:
						pHeld = true;
						break;
				}
			}

			if (controlHeld && data.KeysPressedThisFrame.Contains(Keys.E))
			{
				editorEnabled = !editorEnabled;
			}

			if (!editorEnabled)
			{
				return;
			}

			if (data.KeysPressedThisFrame.Contains(Keys.L))
			{
				platformMode = !platformMode;

				if (!platformMode)
				{
					platformInProgress = false;
				}
			}

			foreach (Keys key in data.KeysPressedThisFrame)
			{
				selectedEntityType = GetEntityType(key);
			}

			if (controlHeld && data.KeysPressedThisFrame.Contains(Keys.S))
			{
				messageSystem.Send(new LevelSaveMessage(platforms));
			}
		}

		private EditableEntityTypes GetEntityType(Keys key)
		{
			switch (key)
			{
				case Keys.F: return EditableEntityTypes.FixedLaserSource;
				case Keys.R: return EditableEntityTypes.LaserReceiver;
				case Keys.K: return EditableEntityTypes.KeyButton;
				case Keys.M: return EditableEntityTypes.Mirror;
				case Keys.O: return EditableEntityTypes.Rotator;
				case Keys.I: return EditableEntityTypes.SlidingDoor;
				case Keys.S: return EditableEntityTypes.Switch;
				case Keys.W: return EditableEntityTypes.Wire;
			}

			return selectedEntityType;
		}

		private void HandleMouse(MouseData data)
		{
			if (!editorEnabled)
			{
				return;
			}

			Vector2 mousePosition = data.WorldPosition;

			int screenWidth = Constants.RoomWidth * Constants.TileSize;
			int screenHeight = Constants.RoomHeight * Constants.TileSize;

			bool inBounds = mousePosition.X >= 0 && mousePosition.X <= screenWidth &&
				mousePosition.Y >= 0 && mousePosition.Y <= screenHeight;
			bool inTileBounds = mousePosition.X > Constants.TileSize && mousePosition.X < screenWidth - Constants.TileSize &&
				mousePosition.Y > Constants.TileSize && mousePosition.Y < screenHeight - Constants.TileSize;

			if (!inBounds)
			{
				return;
			}

			if (platformMode)
			{
				ManagePlatforms(data, mousePosition);

				return;
			}

			if (wire != null)
			{
				EditWire(data, mousePosition, inTileBounds);
			}
			else if (inTileBounds)
			{
				ManageEntities(data, mousePosition);
			}
		}

		private void ManagePlatforms(MouseData data, Vector2 mousePosition)
		{
			Point tileCoordinates = GetTileCoordinates(mousePosition);
			Vector2 tileCenter = TileConvert.ToPixels(tileCoordinates);

			if (platformInProgress)
			{
				platformEnd = tileCenter;

				if (data.RightClickState == ClickStates.PressedThisFrame)
				{
					platformInProgress = false;

					return;
				}

				if (data.LeftClickState == ClickStates.PressedThisFrame)
				{
					float minX = MathHelper.Min(platformStart.X, platformEnd.X);
					float minY = MathHelper.Min(platformStart.Y, platformEnd.Y);
					float maxX = MathHelper.Max(platformStart.X, platformEnd.X);
					float maxY = MathHelper.Max(platformStart.Y, platformEnd.Y);

					Point minPoint = TileConvert.ToTile(new Vector2(minX, minY));
					Point maxPoint = TileConvert.ToTile(new Vector2(maxX, maxY));

					Platform platform = kernel.Get<Platform>();
					platform.Width = maxPoint.X - minPoint.X + 1;
					platform.Height = maxPoint.Y - minPoint.Y + 1;
					platform.LoadPosition = new Vector2(minPoint.X, minPoint.Y);

					PlatformSegment[,] segments = platform.Segments;

					for (int i = 0; i < platform.Height; i++)
					{
						for (int j = 0; j < platform.Width; j++)
						{
							PlatformSegment segment = segments[j, i];
							Point tilePoint = TileConvert.ToTile(segment.Position);

							tiles[tilePoint.X, tilePoint.Y].AttachedEntity = segment;
						}
					}

					platforms.Add(platform);
					platformInProgress = false;
				}
			}
			else if (data.LeftClickState == ClickStates.PressedThisFrame)
			{
				platformInProgress = true;
				platformStart = tileCenter;
				platformEnd = tileCenter;
			}
			else if (data.RightClickState == ClickStates.PressedThisFrame)
			{
				PlatformSegment attachedSegment = tiles[tileCoordinates.X, tileCoordinates.Y].AttachedEntity as PlatformSegment;

				if (attachedSegment != null)
				{
					Platform platform = attachedSegment.Parent;
					Point cornerPoint = TileConvert.ToTile(platform.Position);

					for (int i = cornerPoint.Y; i < cornerPoint.Y + platform.Height; i++)
					{
						for (int j = cornerPoint.X; j < cornerPoint.X + platform.Width; j++)
						{
							tiles[j, i].AttachedEntity = null;
						}
					}

					platform.Dispose();
					platforms.Remove(platform);
				}
			}
		}

		private void EditWire(MouseData data, Vector2 mousePosition, bool inTileBounds)
		{
			List<Vector2> points = wire.Points;
			Vector2 snappedPosition = GetSnappedWirePosition(mousePosition);

			if (data.RightClickState == ClickStates.PressedThisFrame)
			{
				entityMap["Wire"].Remove(wire);
				wire = null;

				return;
			}

			if (data.LeftClickState == ClickStates.PressedThisFrame)
			{
				if (inTileBounds)
				{
					Entity attachedEntity = GetSelectedTile(mousePosition).AttachedEntity;

					IPowered attachedPoweredItem = attachedEntity as IPowered;

					if (attachedPoweredItem != null)
					{
						wire.Points.RemoveAt(wire.Points.Count - 1);
						wire.Points.Add(attachedPoweredItem.WirePosition);

						AbstractPowerSource powerSource1 = wireEntity as AbstractPowerSource;
						AbstractPowerSource powerSource2 = attachedEntity as AbstractPowerSource;
						AbstractPowerSource validSource;

						IPowered otherPoweredItem;

						if (powerSource1 != null)
						{
							validSource = powerSource1;
							otherPoweredItem = attachedPoweredItem;
						}
						else
						{
							validSource = powerSource2;
							otherPoweredItem = wireEntity;
						}

						validSource.TargetIDs.Add(otherPoweredItem.PowerID);
						validSource.PowerTargets.Add(otherPoweredItem);
						validSource.Wires.Add(wire);
						wire.TargetID = validSource.PowerID;
						wire = null;

						return;
					}
				}

				wire.Points.Add(snappedPosition);
			}

			if (points.Count >= 2)
			{
				points.RemoveAt(points.Count - 1);
			}

			wire.Points.Add(snappedPosition);
		}

		private void ManageEntities(MouseData data, Vector2 mousePosition)
		{
			Point tileCoordinates = GetTileCoordinates(mousePosition);
			Tile tile = tiles[tileCoordinates.X, tileCoordinates.Y];

			if (data.LeftClickState == ClickStates.PressedThisFrame)
			{
				if (selectedEntity != null)
				{
					selectedEntity = null;
				}
				else
				{
					if (tile.AttachedEntity != null)
					{
						if (selectedEntityType == EditableEntityTypes.Wire)
						{
							wire = (Wire)CreateEntity();
							wireEntity = (IPowered)tile.AttachedEntity;
							wire.Points.Add(wireEntity.WirePosition);
							entityMap["Wire"].Add(wire);
						}
						else
						{
							selectedEntity = tile.AttachedEntity;
						}
					}
					else if (selectedEntity == null && selectedEntityType != EditableEntityTypes.Wire)
					{
						Entity entity = CreateEntity();
						entity.Position = TileConvert.ToPixels(tileCoordinates);
						tile.AttachedEntity = entity;
						selectedEntity = entity;
					}
				}
			}
			else if (data.RightClickState == ClickStates.PressedThisFrame && tile.AttachedEntity != null)
			{
				tile.AttachedEntity.Dispose();
				tile.AttachedEntity = null;
			}

			if (selectedEntity != null)
			{
				if (pHeld)
				{
					IPowered poweredObject = selectedEntity as IPowered;

					if (poweredObject != null)
					{
						poweredObject.Powered = !poweredObject.Powered;
						selectedEntity = null;
					}
				}
				else
				{
					RotateSelectedEntity(mousePosition);
				}
			}
		}

		private Point GetTileCoordinates(Vector2 mousePosition)
		{
			return new Point((int)mousePosition.X / Constants.TileSize - 1, (int)mousePosition.Y / Constants.TileSize - 1);
		}

		private Vector2 GetSnappedWirePosition(Vector2 mousePosition)
		{
			float x = SnapValue(mousePosition.X);
			float y = SnapValue(mousePosition.Y);

			return new Vector2(x, y);
		}

		private Tile GetSelectedTile(Vector2 mousePosition)
		{
			Point tileCoordinates = GetTileCoordinates(mousePosition);

			return tiles[tileCoordinates.X, tileCoordinates.Y];
		}

		private float SnapValue(float value)
		{
			float leftover = value % Constants.HalfTile;

			if (leftover > Constants.HalfTile)
			{
				return value + (Constants.HalfTile - leftover);
			}

			return value - leftover;
		}

		private void RotateSelectedEntity(Vector2 mousePosition)
		{
			float angle = GameFunctions.ComputeAngle(selectedEntity.Position, mousePosition);

			if (shiftHeld)
			{
				float leftover = Math.Abs(angle % MathHelper.PiOver4);

				if (leftover > PiOver8)
				{
					angle += (MathHelper.PiOver4 - leftover) * Math.Sign(angle);
				}
				else
				{
					angle -= leftover * Math.Sign(angle);
				}
			}

			selectedEntity.Rotation = angle;
		}

		private Entity CreateEntity()
		{
			switch (selectedEntityType)
			{
				case EditableEntityTypes.FixedLaserSource: return kernel.Get<FixedLaserSource>();
				case EditableEntityTypes.LaserReceiver: return kernel.Get<LaserReceiver>();
				case EditableEntityTypes.KeyButton: return kernel.Get<KeyButton>();
				case EditableEntityTypes.Mirror: return kernel.Get<Mirror>();
				case EditableEntityTypes.Rotator: return kernel.Get<Rotator>();
				case EditableEntityTypes.SlidingDoor: return kernel.Get<SlidingDoor>();
				case EditableEntityTypes.Switch: return kernel.Get<Switch>();
				case EditableEntityTypes.Wire: return kernel.Get<Wire>();
			}

			return null;
		}

		public void Render(SpriteBatch sb)
		{
			if (platformMode && platformInProgress)
			{
				int left = (int)MathHelper.Min(platformStart.X, platformEnd.X);
				int right = (int)MathHelper.Max(platformStart.X, platformEnd.X);
				int top = (int)MathHelper.Min(platformStart.Y, platformEnd.Y);
				int bottom = (int)MathHelper.Max(platformStart.Y, platformEnd.Y);

				Rectangle rectangle = new Rectangle(left, top, right - left, bottom - top);

				primitiveDrawer.DrawRectangle(sb, rectangle, Color.Purple);
			}
		}
	}
}
