using Sentient;
using Sentient.Injection;
using Sentient.MeYouKaiju;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Users;

public class ConnectInputToBiplane : MonoBehaviour
{
    private PlayerInputUser _playerInput;

    private BiplaneController _controller;

    private VehicleWeaponService _weaponService;

    [ComponentConstructor]
    private void Construct(PlayerInputUser playerInput, BiplaneController controller, VehicleWeaponService weaponService, IEnumerable<VehicleWeaponSlot> vehicleWeaponSlots)
    {
        _playerInput = playerInput;
        _controller = controller;
        _weaponService = weaponService;
    }

    [UnityCallback]
    private void OnDisable()
    {
        if (_controller)
        {
            _controller.Input_Forward = 0f;
            _controller.Input_Roll = 0f;
            _controller.Input_Pitch = 0f;
            _controller.Input_Yaw = 0f;
        }
    }

    [UnityCallback]
    private void Update()
    {
        if (!_playerInput)
            return;

        InputUser assignedInputUser = _playerInput.AssignedInputUser;
        if (assignedInputUser.valid)
        {
            MykControls.HelicopterActions helicopter = _playerInput.Controls.Helicopter;
            _controller.Input_Forward = helicopter.Thrust.ReadValue<float>();
            _controller.Input_Roll = helicopter.Roll.ReadValue<float>();
            _controller.Input_Pitch = helicopter.Pitch.ReadValue<float>();
            _controller.Input_Yaw = helicopter.Rudder.ReadValue<float>();
            _weaponService.GetWeaponSlot(VehicleWeaponSlotType.Primary)?.SetTriggerState(helicopter.PrimaryFire.IsPressed());
            _weaponService.GetWeaponSlot(VehicleWeaponSlotType.Secondary)?.SetTriggerState(helicopter.SecondaryFire.IsPressed());
            _weaponService.GetWeaponSlot(VehicleWeaponSlotType.Special)?.SetTriggerState(helicopter.Special.IsPressed());
        }
    }
}
