/*++

Module Name:

    public.h

Abstract:

    This module contains the common declarations shared by driver
    and user applications.

Environment:

    user and kernel

--*/

//
// Define an Interface Guid so that apps can find the device and talk to it.
//

DEFINE_GUID (GUID_DEVINTERFACE_,
    0xc29fd6da,0xf6dc,0x408e,0xbd,0xf5,0x28,0x91,0x2d,0x63,0x19,0xd5);
// {c29fd6da-f6dc-408e-bdf5-28912d6319d5}
