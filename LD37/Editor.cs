using System;
using System.Collections.Generic;
using LD37.Entities;
using LD37.Entities.Abstract;
using LD37.Entities.Lasers;
using LD37.Entities.Organization;
using LD37.Input;
using LD37.Interfaces;
using LD37.Messaging;
using LD37.Messaging.Input;
using LD37.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Ninject;

namespace LD37
{
	using EntityMap = Dictionary<string, List<Entity>>;

	internal class Editor : IMessageReceiver
	{
		private const float PiOver8 = MathHelper.PiOver4 / 2;

		private enum EditableEntityTypes
		{
			FixedLaserSource,
			LaserReceiver,
			KeyButton,
			Mirror,
			Switch,
			Wire
		}

		private Tile[,] tiles;
		private Entity selectedEntity;
		private MessageSystem messageSystem;
		private StandardKernel kernel;
		private EntityMap entityMap;
		private Wire wire;

		private IPowered wireEntity;
		private EditableEntityTypes selectedEntityType;

		private bool shiftHeld;
		private bool pHeld;

		public Editor(MessageSystem messageSystem, Scene scene, StandardKernel kernel)
		{
			this.messageSystem = messageSystem;
			this.kernel = kernel;

			tiles = scene.RetrieveTiles();
			entityMap = scene.LayerMap["Primary"].EntityMap;

			messageSystem.Subscribe(MessageTypes.Keyboard, this);
			messageSystem.Subscribe(MessageTypes.Mouse, this);
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
			foreach (Keys key in data.KeysPressedThisFrame)
			{
				selectedEntityType = GetEntityType(key);
			}

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

			if (controlHeld && data.KeysPressedThisFrame.Contains(Keys.S))
			{
				messageSystem.Send(new GameMessage(MessageTypes.LevelSave));
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
				case Keys.S: return EditableEntityTypes.Switch;
				case Keys.W: return EditableEntityTypes.Wire;
			}

			return selectedEntityType;
		}

		private void HandleMouse(MouseData data)
		{
			Vector2 mousePosition = data.WorldPosition;

			int screenRight = Constants.RoomWidth * Constants.TileSize;
			int screenHeight = Constants.RoomHeight * Constants.TileSize;

			if (mousePosition.X <= Constants.TileSize || mousePosition.X >= screenRight - Constants.TileSize ||
			    mousePosition.Y <= Constants.TileSize || mousePosition.Y >= screenHeight - Constants.TileSize)
			{
				return;
			}

			if (wire != null)
			{
				EditWire(data, mousePosition);
			}
			else
			{
				ManageEntities(data, mousePosition);
			}
		}

		private void EditWire(MouseData data, Vector2 mousePosition)
		{
			List<Vector2> points = wire.Points;
			Vector2 snappedPosition = GetSnappedWirePosition(mousePosition);

			if (data.LeftClickState == ClickStates.PressedThisFrame)
			{
				Entity attachedEntity = GetSelectedTile(mousePosition).AttachedEntity;

				IPowered poweredEntity = attachedEntity as IPowered;

				if (poweredEntity != null)
				{
					wire.Points.RemoveAt(wire.Points.Count - 1);
					wire.Points.Add(poweredEntity.WirePosition);

					AbstractPowerSource powerSource1 = wireEntity as AbstractPowerSource;
					AbstractPowerSource powerSource2 = attachedEntity as AbstractPowerSource;

					if (powerSource1 != null)
					{
						powerSource1.TargetIDs.Add(poweredEntity.PowerID);
						powerSource1.PowerTargets.Add(poweredEntity);
					}
					else
					{
						powerSource2.TargetIDs.Add(wireEntity.PowerID);
						powerSource2.PowerTargets.Add(wireEntity);
					}

					wire = null;

					return;
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
				case EditableEntityTypes.Switch: return kernel.Get<Switch>();
				case EditableEntityTypes.Wire: return kernel.Get<Wire>();
			}

			return null;
		}
	}
}
