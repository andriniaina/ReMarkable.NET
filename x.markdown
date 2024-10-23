# The reMarkable 2 Framebuffer

As opposed to the rM1, the rM2 does not use the embedded EPDC of the i.MX 7. Instead the Electrophoretic Display is connected directly to the LCD controller. This means all stuff that the EPDC would normally do is now done in software, which is mainly writing the correct (temperature dependent) waveform to the framebuffer in order to get the display to show what we want.

The software that implements the EPDC in software is called SWTCON by the reMarkable team. It's closed sources which makes it difficult to get custom applications working. The SWTCON is included in both the Xochitl and remarkable-shutdown executables. It's compiled in statically and only one can run at the same time. All of the analysis that follows was gotten be reverse-engineering the remarkable-shutdown executable of firmware version 2.4.1.30.

## Display QImage

The SWTCON is created by a singleton that wraps it in a Qt QImage. The singleton is created and can be retrieved by a getInstance method at 0x00021f54. This method will return the instance if it exists and otherwise create one by calling 0x00021d64 (MakeInstance). The MakeInstance method initializes some fields but ultimately it calls a function that creates the SWTCON threads. It then waits until the threads are initialized before returning (function at 0x00022f80).

## SWTCON threads
The function at 0x00023ca4 creates two threads after reading the waveform files. One is called the 'vsync and flip thread' by the debug messages, the other is called the 'generator thread'.

The vsync thread seems to be mainly responsible for updating the 'phase' of the framebuffer. It first initializes the display in a similar way to uboot (epd_display_init in uboot source). It will then display different 'phases' of the framebuffer image by using the pan ioctl. It seems that there are 16 virtual framebuffers allocated which are panned through to get the different waveform phases on the screen.

The generator thread listens for updates and processes commands sent through a linked list. It also has a second private linked list where some command buffers get put on after being processed.

## Update function
The function at 0x00021a34 sends updates to the generator thread. It takes a singleton, a rectangle, a waveform id and some update flags as arguments. The function will transform the waveform id before calling the function that actually sends a message to the generator thread. The recognized waveforms are:

| Waveform |	Out |
|--|--|
| 0     | 2 |
| 1	    | 0 |
| 2	    | 1 |
| 3	    | 2 |
| 4-7	| error |
| 8	    | 1 |
There seem to be at least two update flags. The second bit determines whether the update is blocking or not.

The function at 0x00023554 will take the transformed waveform, rectangle to update and flags and allocate a command buffer. This buffer is then put on the linked list and the generator thread is notified. The actual structure of these command buffers is unknown.

## Current workaround
The current workaround to get rm1 apps working on the rm2 is using the remarkable2-framebuffer project. This project consists of a server and a client wrapper. The server wrapper wraps remarkable-shutdown and starts a listening process that call to the update function. The client process hooks a target app to create a fake framebuffer, the fake framebuffer will forward commands to the server process by using shared memory and message queues.

This website uses cookies for visitor traffic analysis. By using the website, you agree with storing the cookies on your computer.OKMore information
