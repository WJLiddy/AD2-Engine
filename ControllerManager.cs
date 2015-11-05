using SlimDX;
using SlimDX.DirectInput;
using System.Collections.Generic;

public class ControllerManager
{
    //Axis Range for controller [-AXIS_RANGE...AXIS_RANGE] represents all values of an axis
    //TODO: Decide if 1000 makes sense as a default value.
    public static readonly int AXIS_RANGE = 1000;

    //List of all joysticks connected to the system.
    private LinkedList<Joystick> joysticksConnected;

    public ControllerManager()
    {
        joysticksConnected = new LinkedList<Joystick>();
        connectJoysticks();
    }

    public void connectJoysticks()
    {
        // make sure that DirectInput has been initialized
        DirectInput dinput = new DirectInput();
        // search for devices
        foreach (DeviceInstance device in dinput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly))
        {
            // create the device
            try
            {
                joysticksConnected.AddLast(new Joystick(dinput, device.InstanceGuid));
            }
            catch (DirectInputException)
            {
                Utils.log("Warning: Joystick did not init (DirectInputException)");
            }
        }

        Utils.log("joysticks connected: " +  joysticksConnected.Count);

        //Set the axises of all of the analog sticks, then claim the joystick
        foreach (Joystick joystick in joysticksConnected)
        { 
            foreach (DeviceObjectInstance deviceObject in joystick.GetObjects())
            {
                if ((deviceObject.ObjectType & ObjectDeviceType.Axis) != 0)
                    joystick.GetObjectPropertiesById((int)deviceObject.ObjectType).SetRange(-AXIS_RANGE, AXIS_RANGE);
            }
            joystick.Acquire();
        }

    }

    public JoystickState[] getState()
    {
        JoystickState[] states = new JoystickState[joysticksConnected.Count];

        //gather inputs from all the joystics
        int i = 0;
        foreach (Joystick joystick in joysticksConnected)
        {
            if (joystick.Acquire().IsFailure)
                continue;

            if (joystick.Poll().IsFailure)
                continue;

            states[i] = joystick.GetCurrentState();
            i++;
  
            if (Result.Last.IsFailure)
                continue;
        }
        return states;
    }
}