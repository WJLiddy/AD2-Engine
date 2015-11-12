using SlimDX;
using SlimDX.DirectInput;
using System.Collections.Generic;

public class ControllerManager
{
    //Axis Range for controller [-AXIS_RANGE...AXIS_RANGE] represents all values of an axis
    //TODO: Decide if 1000 makes sense as a default value.
    public static readonly int AxisRange = 1000;

    //List of all joysticks connected to the system.
    private LinkedList<Joystick> JoysticksConnected;

    public ControllerManager()
    {
        JoysticksConnected = new LinkedList<Joystick>();
        ConnectJoysticks();
    }

    public void ConnectJoysticks()
    {
        // make sure that DirectInput has been initialized
        DirectInput dinput = new DirectInput();
        // search for devices
        foreach (DeviceInstance device in dinput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly))
        {
            // create the device
            try
            {
                JoysticksConnected.AddLast(new Joystick(dinput, device.InstanceGuid));
            }
            catch (DirectInputException)
            {
                Utils.Log("Warning: Joystick did not init (DirectInputException)");
            }
        }

        Utils.Log("joysticks connected: " +  JoysticksConnected.Count);

        //Set the axises of all of the analog sticks, then claim the joystick
        foreach (Joystick joystick in JoysticksConnected)
        { 
            foreach (DeviceObjectInstance deviceObject in joystick.GetObjects())
            {
                if ((deviceObject.ObjectType & ObjectDeviceType.Axis) != 0)
                    joystick.GetObjectPropertiesById((int)deviceObject.ObjectType).SetRange(-AxisRange, AxisRange);
            }
            joystick.Acquire();
        }

    }

    public JoystickState[] GetState()
    {
        JoystickState[] states = new JoystickState[JoysticksConnected.Count];

        //gather inputs from all the joystics
        int i = 0;
        foreach (Joystick joystick in JoysticksConnected)
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