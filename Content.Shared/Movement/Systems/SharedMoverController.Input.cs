// SPDX-FileCopyrightText: 2020 ComicIronic <comicironic@gmail.com>
// SPDX-FileCopyrightText: 2020 GlassEclipse <32942106+GlassEclipse@users.noreply.github.com>
// SPDX-FileCopyrightText: 2020 Jackson Lewis <inquisitivepenguin@protonmail.com>
// SPDX-FileCopyrightText: 2020 Vince <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto <zddm@outlook.es>
// SPDX-FileCopyrightText: 2021 Acruid <shatter66@gmail.com>
// SPDX-FileCopyrightText: 2021 Javier Guardia Fernández <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Metal Gear Sloth <metalgearsloth@gmail.com>
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto <gradientvera@outlook.com>
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto <zddm@outlook.es>
// SPDX-FileCopyrightText: 2021 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 collinlunn <60152240+collinlunn@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 metalgearsloth <comedian_vs_clown@hotmail.com>
// SPDX-FileCopyrightText: 2021 metalgearsloth <metalgearsloth@gmail.com>
// SPDX-FileCopyrightText: 2022 Rane <60792108+Elijahrane@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 0x6273 <0x40@keemail.me>
// SPDX-FileCopyrightText: 2024 Aidenkrz <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2024 Ed <96445749+TheShuEd@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 ElectroJr <leonsfriedrich@gmail.com>
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2024 Plykiya <58439124+Plykiya@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 SlamBamActionman <83650252+SlamBamActionman@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2024 plykiya <plykiya@protonmail.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 GoobBot <uristmchands@proton.me>
// SPDX-FileCopyrightText: 2025 Kyle Tyo <36606155+VerinSenpai@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 MarkerWicker <markerWicker@proton.me>
// SPDX-FileCopyrightText: 2025 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Pronana@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 gluesniffler <159397573+gluesniffler@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Numerics;
using Content.Shared.Alert;
using Content.Shared.CCVar;
using Content.Shared.Follower.Components;
using Content.Shared.Input;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Robust.Shared.GameStates;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.Map.Components;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using Robust.Shared.Maths; // Shitmed Change

namespace Content.Shared.Movement.Systems
{
    /// <summary>
    ///     Handles converting inputs into movement.
    /// </summary>
    public abstract partial class SharedMoverController
    {
        public bool CameraRotationLocked { get; set; }

        public static ProtoId<AlertPrototype> WalkingAlert = "Walking";

        private void InitializeInput()
        {
            var moveUpCmdHandler = new MoverDirInputCmdHandler(this, Direction.North);
            var moveLeftCmdHandler = new MoverDirInputCmdHandler(this, Direction.West);
            var moveRightCmdHandler = new MoverDirInputCmdHandler(this, Direction.East);
            var moveDownCmdHandler = new MoverDirInputCmdHandler(this, Direction.South);

            CommandBinds.Builder
                .Bind(EngineKeyFunctions.MoveUp, moveUpCmdHandler)
                .Bind(EngineKeyFunctions.MoveLeft, moveLeftCmdHandler)
                .Bind(EngineKeyFunctions.MoveRight, moveRightCmdHandler)
                .Bind(EngineKeyFunctions.MoveDown, moveDownCmdHandler)
                .Bind(EngineKeyFunctions.Walk, new WalkInputCmdHandler(this))
                .Bind(EngineKeyFunctions.CameraRotateLeft, new CameraRotateInputCmdHandler(this, Direction.East))
                .Bind(EngineKeyFunctions.CameraRotateRight, new CameraRotateInputCmdHandler(this, Direction.West))
                .Bind(EngineKeyFunctions.CameraReset, new CameraResetInputCmdHandler(this))
                // TODO: Relay
                // Shuttle
                .Bind(ContentKeyFunctions.ShuttleStrafeUp, new ShuttleInputCmdHandler(this, ShuttleButtons.StrafeUp))
                .Bind(ContentKeyFunctions.ShuttleStrafeLeft, new ShuttleInputCmdHandler(this, ShuttleButtons.StrafeLeft))
                .Bind(ContentKeyFunctions.ShuttleStrafeRight, new ShuttleInputCmdHandler(this, ShuttleButtons.StrafeRight))
                .Bind(ContentKeyFunctions.ShuttleStrafeDown, new ShuttleInputCmdHandler(this, ShuttleButtons.StrafeDown))
                .Bind(ContentKeyFunctions.ShuttleRotateLeft, new ShuttleInputCmdHandler(this, ShuttleButtons.RotateLeft))
                .Bind(ContentKeyFunctions.ShuttleRotateRight, new ShuttleInputCmdHandler(this, ShuttleButtons.RotateRight))
                .Bind(ContentKeyFunctions.ShuttleBrake, new ShuttleInputCmdHandler(this, ShuttleButtons.Brake))
                .Register<SharedMoverController>();

            SubscribeLocalEvent<InputMoverComponent, ComponentInit>(OnInputInit);
            SubscribeLocalEvent<InputMoverComponent, ComponentGetState>(OnMoverGetState);
            SubscribeLocalEvent<InputMoverComponent, ComponentHandleState>(OnMoverHandleState);
            SubscribeLocalEvent<InputMoverComponent, EntParentChangedMessage>(OnInputParentChange);

            SubscribeLocalEvent<FollowedComponent, EntParentChangedMessage>(OnFollowedParentChange);

            Subs.CVar(_configManager, CCVars.CameraRotationLocked, obj => CameraRotationLocked = obj, true);
            Subs.CVar(_configManager, CCVars.GameDiagonalMovement, value => DiagonalMovementEnabled = value, true);
        }

        /// <summary>
        /// Gets the buttons held with opposites cancelled out.
        /// </summary>
        public static MoveButtons GetNormalizedMovement(MoveButtons buttons)
        {
            var oldMovement = buttons;

            if ((oldMovement & (MoveButtons.Left | MoveButtons.Right)) == (MoveButtons.Left | MoveButtons.Right))
            {
                oldMovement &= ~MoveButtons.Left;
                oldMovement &= ~MoveButtons.Right;
            }

            if ((oldMovement & (MoveButtons.Up | MoveButtons.Down)) == (MoveButtons.Up | MoveButtons.Down))
            {
                oldMovement &= ~MoveButtons.Up;
                oldMovement &= ~MoveButtons.Down;
            }

            return oldMovement;
        }

        protected void SetMoveInput(Entity<InputMoverComponent> entity, MoveButtons buttons)
        {
            if (entity.Comp.HeldMoveButtons == buttons)
                return;

            // Relay the fact we had any movement event.
            // TODO: Ideally we'd do these in a tick instead of out of sim.
            // Shitmed Change Start
            Vector2 vector2 = DirVecForButtons(buttons);
            Vector2i vector2i = new Vector2i((int) vector2.X, (int) vector2.Y);
            Direction dir = (vector2i == Vector2i.Zero) ? Direction.Invalid : vector2i.AsDirection();
            var moveEvent = new MoveInputEvent(entity, buttons, dir, buttons != 0);
            // Shitmed Change End
            entity.Comp.HeldMoveButtons = buttons;
            RaiseLocalEvent(entity, ref moveEvent);
            Dirty(entity, entity.Comp);

            var ev = new SpriteMoveEvent(entity.Comp.HeldMoveButtons != MoveButtons.None);
            RaiseLocalEvent(entity, ref ev);
        }

        private void OnMoverHandleState(Entity<InputMoverComponent> entity, ref ComponentHandleState args)
        {
            if (args.Current is not InputMoverComponentState state)
                return;

            // Handle state
            entity.Comp.LerpTarget = state.LerpTarget;
            entity.Comp.RelativeRotation = state.RelativeRotation;
            entity.Comp.TargetRelativeRotation = state.TargetRelativeRotation;
            entity.Comp.CanMove = state.CanMove;
            entity.Comp.RelativeEntity = EnsureEntity<InputMoverComponent>(state.RelativeEntity, entity.Owner);
            entity.Comp.DefaultSprinting = state.DefaultSprinting;

            // Reset
            entity.Comp.LastInputTick = GameTick.Zero;
            entity.Comp.LastInputSubTick = 0;
             // Shitmed Change Start
            Vector2 vector2 = DirVecForButtons(entity.Comp.HeldMoveButtons);
            Vector2i vector2i = new Vector2i((int) vector2.X, (int) vector2.Y);
            Direction dir = (vector2i == Vector2i.Zero) ? Direction.Invalid : vector2i.AsDirection();
            // Shitmed Change End
            if (entity.Comp.HeldMoveButtons != state.HeldMoveButtons)
            {
                var moveEvent = new MoveInputEvent(entity, entity.Comp.HeldMoveButtons, dir, state.HeldMoveButtons != 0); // Shitmed Change
                entity.Comp.HeldMoveButtons = state.HeldMoveButtons;
                RaiseLocalEvent(entity.Owner, ref moveEvent);

                var ev = new SpriteMoveEvent(entity.Comp.HeldMoveButtons != MoveButtons.None);
                RaiseLocalEvent(entity, ref ev);
            }
        }

        private void OnMoverGetState(Entity<InputMoverComponent> entity, ref ComponentGetState args)
        {
            args.State = new InputMoverComponentState()
            {
                CanMove = entity.Comp.CanMove,
                RelativeEntity = GetNetEntity(entity.Comp.RelativeEntity),
                LerpTarget = entity.Comp.LerpTarget,
                HeldMoveButtons = entity.Comp.HeldMoveButtons,
                RelativeRotation = entity.Comp.RelativeRotation,
                TargetRelativeRotation = entity.Comp.TargetRelativeRotation,
                DefaultSprinting = entity.Comp.DefaultSprinting
            };
        }

        private void ShutdownInput()
        {
            CommandBinds.Unregister<SharedMoverController>();
        }

        public bool DiagonalMovementEnabled { get; private set; }

        protected virtual void HandleShuttleInput(EntityUid uid, ShuttleButtons button, ushort subTick, bool state) {}

        public void RotateCamera(EntityUid uid, Angle angle)
        {
            if (CameraRotationLocked || !MoverQuery.TryGetComponent(uid, out var mover))
                return;

            mover.TargetRelativeRotation += angle;
            Dirty(uid, mover);
        }

        public void ResetCamera(EntityUid uid)
        {
            if (CameraRotationLocked ||
                !MoverQuery.TryGetComponent(uid, out var mover))
            {
                return;
            }

            // Shitmed Change Start
            var xform = XformQuery.GetComponent(uid);
            if (TryComp(uid, out RelayInputMoverComponent? relay)
                 && TryComp(relay.RelayEntity, out TransformComponent? relayXform)
                 && MoverQuery.TryGetComponent(relay.RelayEntity, out var relayMover))
                xform = relayXform;

            // If we updated parent then cancel the accumulator and force it now.
            if (!TryUpdateRelative(uid, mover, xform) && mover.TargetRelativeRotation.Equals(Angle.Zero))
                return;
            // Shitmed Change End

            mover.LerpTarget = TimeSpan.Zero;
            mover.TargetRelativeRotation = Angle.Zero;
            Dirty(uid, mover);
        }

        private bool TryUpdateRelative(EntityUid uid, InputMoverComponent mover, TransformComponent xform)
        {
            var relative = xform.GridUid;
            relative ??= xform.MapUid;

            // So essentially what we want:
            // 1. If we go from grid to map then preserve our rotation and continue as usual
            // 2. If we go from grid -> grid then (after lerp time) snap to nearest cardinal (probably imperceptible)
            // 3. If we go from map -> grid then (after lerp time) snap to nearest cardinal

            if (mover.RelativeEntity.Equals(relative))
                return false;

            // Okay need to get our old relative rotation with respect to our new relative rotation
            // e.g. if we were right side up on our current grid need to get what that is on our new grid.
            var oldRelativeRot = Angle.Zero;
            var relativeRot = Angle.Zero;

            // Get our current relative rotation
            if (XformQuery.TryGetComponent(mover.RelativeEntity, out var oldRelativeXform))
            {
                oldRelativeRot = _transform.GetWorldRotation(oldRelativeXform);
            }

            if (XformQuery.TryGetComponent(relative, out var relativeXform))
            {
                // This is our current rotation relative to our new parent.
                relativeRot = _transform.GetWorldRotation(relativeXform);
            }

            var diff = relativeRot - oldRelativeRot;

            // If we're going from a grid -> map then preserve the relative rotation so it's seamless if they go into space and back.
            if (MapQuery.HasComp(relative) && MapGridQuery.HasComp(mover.RelativeEntity))
            {
                mover.TargetRelativeRotation -= diff;
            }
            // Snap to nearest cardinal if map -> grid or grid -> grid
            else if (MapGridQuery.HasComp(relative) && (MapQuery.HasComp(mover.RelativeEntity) || MapGridQuery.HasComp(mover.RelativeEntity)))
            {
                var targetDir = mover.TargetRelativeRotation - diff;
                targetDir = targetDir.GetCardinalDir().ToAngle().Reduced();
                mover.TargetRelativeRotation = targetDir;
            }

            // Preserve target rotation in relation to the new parent.
            // Regardless of what the target is don't want the eye to move at all (from the player's perspective).
            mover.RelativeRotation -= diff;

            mover.RelativeEntity = relative;
            Dirty(uid, mover);
            return true;
        }

        public Angle GetParentGridAngle(InputMoverComponent mover)
        {
            var rotation = mover.RelativeRotation;

            if (XformQuery.TryGetComponent(mover.RelativeEntity, out var relativeXform))
                return _transform.GetWorldRotation(relativeXform) + rotation;

            return rotation;
        }

        private void OnFollowedParentChange(Entity<FollowedComponent> entity, ref EntParentChangedMessage args)
        {
            foreach (var foll in entity.Comp.Following)
            {
                if (!MoverQuery.TryGetComponent(foll, out var mover))
                    continue;

                var ev = new EntParentChangedMessage(foll, null, args.OldMapId, XformQuery.GetComponent(foll));
                OnInputParentChange((foll, mover), ref ev);
            }
        }

        private void OnInputParentChange(Entity<InputMoverComponent> entity, ref EntParentChangedMessage args)
        {
            // If we change our grid / map then delay updating our LastGridAngle.
            var relative = args.Transform.GridUid;
            relative ??= args.Transform.MapUid;

            if (entity.Comp.LifeStage < ComponentLifeStage.Running)
            {
                entity.Comp.RelativeEntity = relative;
                Dirty(entity.Owner, entity.Comp);
                return;
            }

            var oldMapId = args.OldMapId;
            var mapId = args.Transform.MapUid;

            // If we change maps then reset eye rotation entirely.
            if (oldMapId != mapId)
            {
                entity.Comp.RelativeEntity = relative;
                entity.Comp.TargetRelativeRotation = Angle.Zero;
                entity.Comp.RelativeRotation = Angle.Zero;
                entity.Comp.LerpTarget = TimeSpan.Zero;
                Dirty(entity.Owner, entity.Comp);
                return;
            }

            // If we go on a grid and back off then just reset the accumulator.
            if (relative == entity.Comp.RelativeEntity)
            {
                if (entity.Comp.LerpTarget >= Timing.CurTime)
                {
                    entity.Comp.LerpTarget = TimeSpan.Zero;
                    Dirty(entity.Owner, entity.Comp);
                }

                return;
            }

            entity.Comp.LerpTarget = TimeSpan.FromSeconds(InputMoverComponent.LerpTime) + Timing.CurTime;
            Dirty(entity.Owner, entity.Comp);
        }

        private void HandleDirChange(EntityUid entity, Direction dir, ushort subTick, bool state)
        {
            // Relayed movement just uses the same keybinds given we're moving the relayed entity
            // the same as us.

            // TODO: Should move this into HandleMobMovement itself.
            if (TryComp<RelayInputMoverComponent>(entity, out var relayMover))
            {
                DebugTools.Assert(relayMover.RelayEntity != entity);
                DebugTools.AssertNotNull(relayMover.RelayEntity);

                if (MoverQuery.TryGetComponent(entity, out var mover))
                    SetMoveInput((entity, mover), MoveButtons.None);

                if (!_mobState.IsIncapacitated(entity))
                    HandleDirChange(relayMover.RelayEntity, dir, subTick, state);

                return;
            }

            if (!MoverQuery.TryGetComponent(entity, out var moverComp))
                return;

            // Shitmed Change Start
            var moverEntity = new Entity<InputMoverComponent>(entity, moverComp);
            var moveEvent = new MoveInputEvent(moverEntity, moverComp.HeldMoveButtons, dir, state);
            RaiseLocalEvent(entity, ref moveEvent);
            // Shitmed Change End

            // For stuff like "Moving out of locker" or the likes
            // We'll relay a movement input to the parent.
            if (_container.IsEntityInContainer(entity) &&
                TryComp(entity, out TransformComponent? xform) &&
                xform.ParentUid.IsValid() &&
                _mobState.IsAlive(entity))
            {
                var relayMoveEvent = new ContainerRelayMovementEntityEvent(entity);
                RaiseLocalEvent(xform.ParentUid, ref relayMoveEvent);
            }

            SetVelocityDirection((entity, moverComp), dir, subTick, state);
        }

        private void OnInputInit(Entity<InputMoverComponent> entity, ref ComponentInit args)
        {
            var xform = Transform(entity.Owner);

            if (!xform.ParentUid.IsValid())
                return;

            entity.Comp.RelativeEntity = xform.GridUid ?? xform.MapUid;
            entity.Comp.TargetRelativeRotation = Angle.Zero;
        }

        private void HandleRunChange(EntityUid uid, ushort subTick, bool walking)
        {
            MoverQuery.TryGetComponent(uid, out var moverComp);

            if (TryComp<RelayInputMoverComponent>(uid, out var relayMover))
            {
                // if we swap to relay then stop our existing input if we ever change back.
                if (moverComp != null)
                {
                    SetMoveInput((uid, moverComp), MoveButtons.None);
                }

                HandleRunChange(relayMover.RelayEntity, subTick, walking);
                return;
            }

            if (moverComp == null) return;

            SetSprinting((uid, moverComp), subTick, walking);
        }

        public (Vector2 Walking, Vector2 Sprinting) GetVelocityInput(InputMoverComponent mover)
        {
            if (!Timing.InSimulation)
            {
                // Outside of simulation we'll be running client predicted movement per-frame.
                // So return a full-length vector as if it's a full tick.
                // Physics system will have the correct time step anyways.
                var immediateDir = DirVecForButtons(mover.HeldMoveButtons);
                return mover.Sprinting ? (Vector2.Zero, immediateDir) : (immediateDir, Vector2.Zero);
            }

            Vector2 walk;
            Vector2 sprint;
            float remainingFraction;

            if (Timing.CurTick > mover.LastInputTick)
            {
                walk = Vector2.Zero;
                sprint = Vector2.Zero;
                remainingFraction = 1;
            }
            else
            {
                walk = mover.CurTickWalkMovement;
                sprint = mover.CurTickSprintMovement;
                remainingFraction = (ushort.MaxValue - mover.LastInputSubTick) / (float) ushort.MaxValue;
            }

            var curDir = DirVecForButtons(mover.HeldMoveButtons) * remainingFraction;

            if (mover.Sprinting)
            {
                sprint += curDir;
            }
            else
            {
                walk += curDir;
            }

            // Logger.Info($"{curDir}{walk}{sprint}");
            return (walk, sprint);
        }

        /// <summary>
        ///     Toggles one of the four cardinal directions. Each of the four directions are
        ///     composed into a single direction vector, <see cref="VelocityDir"/>. Enabling
        ///     opposite directions will cancel each other out, resulting in no direction.
        /// </summary>
        public void SetVelocityDirection(Entity<InputMoverComponent> entity, Direction direction, ushort subTick, bool enabled)
        {
            // Logger.Info($"[{_gameTiming.CurTick}/{subTick}] {direction}: {enabled}");

            var bit = direction switch
            {
                Direction.East => MoveButtons.Right,
                Direction.North => MoveButtons.Up,
                Direction.West => MoveButtons.Left,
                Direction.South => MoveButtons.Down,
                _ => throw new ArgumentException(nameof(direction))
            };

            SetMoveInput(entity, subTick, enabled, bit);
        }

        private void SetMoveInput(Entity<InputMoverComponent> entity, ushort subTick, bool enabled, MoveButtons bit)
        {
            // Modifies held state of a movement button at a certain sub tick and updates current tick movement vectors.
            ResetSubtick(entity.Comp);

            if (subTick >= entity.Comp.LastInputSubTick)
            {
                var fraction = (subTick - entity.Comp.LastInputSubTick) / (float) ushort.MaxValue;

                ref var lastMoveAmount = ref entity.Comp.Sprinting ? ref entity.Comp.CurTickSprintMovement : ref entity.Comp.CurTickWalkMovement;

                lastMoveAmount += DirVecForButtons(entity.Comp.HeldMoveButtons) * fraction;

                entity.Comp.LastInputSubTick = subTick;
            }

            var buttons = entity.Comp.HeldMoveButtons;

            if (enabled)
            {
                buttons |= bit;
            }
            else
            {
                buttons &= ~bit;
            }

            SetMoveInput(entity, buttons);
        }

        private void ResetSubtick(InputMoverComponent component)
        {
            if (Timing.CurTick <= component.LastInputTick) return;

            component.CurTickWalkMovement = Vector2.Zero;
            component.CurTickSprintMovement = Vector2.Zero;
            component.LastInputTick = Timing.CurTick;
            component.LastInputSubTick = 0;
        }

        public virtual void SetSprinting(Entity<InputMoverComponent> entity, ushort subTick, bool walking)
        {
            // Logger.Info($"[{_gameTiming.CurTick}/{subTick}] Sprint: {enabled}");

            SetMoveInput(entity, subTick, walking, MoveButtons.Walk);
        }

        /// <summary>
        ///     Retrieves the normalized direction vector for a specified combination of movement keys.
        /// </summary>
        private Vector2 DirVecForButtons(MoveButtons buttons)
        {
            // key directions are in screen coordinates
            // _moveDir is in world coordinates
            // if the camera is moved, this needs to be changed

            var x = 0;
            x -= HasFlag(buttons, MoveButtons.Left) ? 1 : 0;
            x += HasFlag(buttons, MoveButtons.Right) ? 1 : 0;

            var y = 0;
            if (DiagonalMovementEnabled || x == 0)
            {
                y -= HasFlag(buttons, MoveButtons.Down) ? 1 : 0;
                y += HasFlag(buttons, MoveButtons.Up) ? 1 : 0;
            }

            var vec = new Vector2(x, y);

            // can't normalize zero length vector
            if (vec.LengthSquared() > 1.0e-6)
            {
                // Normalize so that diagonals aren't faster or something.
                vec = vec.Normalized();
            }

            return vec;
        }

        private static bool HasFlag(MoveButtons buttons, MoveButtons flag)
        {
            return (buttons & flag) == flag;
        }

        private sealed class CameraRotateInputCmdHandler : InputCmdHandler
        {
            private readonly SharedMoverController _controller;
            private readonly Angle _angle;

            public CameraRotateInputCmdHandler(SharedMoverController controller, Direction direction)
            {
                _controller = controller;
                _angle = direction.ToAngle();
            }

            public override bool HandleCmdMessage(IEntityManager entManager, ICommonSession? session, IFullInputCmdMessage message)
            {
                if (session?.AttachedEntity == null) return false;

                if (message.State != BoundKeyState.Up)
                    return false;

                _controller.RotateCamera(session.AttachedEntity.Value, _angle);
                return false;
            }
        }

        private sealed class CameraResetInputCmdHandler : InputCmdHandler
        {
            private readonly SharedMoverController _controller;

            public CameraResetInputCmdHandler(SharedMoverController controller)
            {
                _controller = controller;
            }

            public override bool HandleCmdMessage(IEntityManager entManager, ICommonSession? session, IFullInputCmdMessage message)
            {
                if (session?.AttachedEntity == null) return false;

                if (message.State != BoundKeyState.Up)
                    return false;

                _controller.ResetCamera(session.AttachedEntity.Value);
                return false;
            }
        }

        private sealed class MoverDirInputCmdHandler : InputCmdHandler
        {
            private readonly SharedMoverController _controller;
            private readonly Direction _dir;

            public MoverDirInputCmdHandler(SharedMoverController controller, Direction dir)
            {
                _controller = controller;
                _dir = dir;
            }

            public override bool HandleCmdMessage(IEntityManager entManager, ICommonSession? session, IFullInputCmdMessage message)
            {
                if (session?.AttachedEntity == null) return false;

                _controller.HandleDirChange(session.AttachedEntity.Value, _dir, message.SubTick, message.State == BoundKeyState.Down);
                return false;
            }
        }

        private sealed class WalkInputCmdHandler : InputCmdHandler
        {
            private SharedMoverController _controller;

            public WalkInputCmdHandler(SharedMoverController controller)
            {
                _controller = controller;
            }

            public override bool HandleCmdMessage(IEntityManager entManager, ICommonSession? session, IFullInputCmdMessage message)
            {
                if (session?.AttachedEntity == null) return false;

                _controller.HandleRunChange(session.AttachedEntity.Value, message.SubTick, message.State == BoundKeyState.Down);
                return false;
            }
        }

        private sealed class ShuttleInputCmdHandler : InputCmdHandler
        {
            private readonly SharedMoverController _controller;
            private readonly ShuttleButtons _button;

            public ShuttleInputCmdHandler(SharedMoverController controller, ShuttleButtons button)
            {
                _controller = controller;
                _button = button;
            }

            public override bool HandleCmdMessage(IEntityManager entManager, ICommonSession? session, IFullInputCmdMessage message)
            {
                if (session?.AttachedEntity == null) return false;

                _controller.HandleShuttleInput(session.AttachedEntity.Value, _button, message.SubTick, message.State == BoundKeyState.Down);
                return false;
            }
        }
    }

    [Flags]
    [Serializable, NetSerializable]
    public enum MoveButtons : byte
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 4,
        Right = 8,
        Walk = 16,
        AnyDirection = Up | Down | Left | Right,
    }

    [Flags]
    public enum ShuttleButtons : byte
    {
        None = 0,
        StrafeUp = 1 << 0,
        StrafeDown = 1 << 1,
        StrafeLeft = 1 << 2,
        StrafeRight = 1 << 3,
        RotateLeft = 1 << 4,
        RotateRight = 1 << 5,
        Brake = 1 << 6,
    }

}
