# TFG | Drone Gesture-Based Drone Control in VR

This project explores whether gesture based drone controls can be a viable alternative to traditional physical controllers for beginner users, and in what situations each approach works best. To answer this question, 3 gesture based control systems were designed and implemented within a custom VR drone simulator built in Unity for the Meta Quest 3 and compared against a PS4 gamepad as the traditional baseline.

Controllers
-----------

- Pinch controller — the user moves their hand in 3D space and the drone follows that movement directly, using a pinch gesture to activate and deactivate control.
- Microgesture controller — uses discrete finger gestures such as taps and slides to trigger impulse and continuous movement directions, as an alternative to traditional analog input.
- Wrist controller — maps the rotations of the user's wrists to the drone's movement axes (pitch, yaw, throttle and roll), serving as a closer analog to a continuous joystick controller.
- PS4 gamepad — serves as the control baseline, using the standard DJI-style stick layout.

Courses
-------

Three drone courses were designed, each targeting a different piloting skill:

- Rings — speed and agility, navigating through a set of rings as fast as possible.
- Obstacle — precision and navigation, traversing a tight corridor with obstacles.
- Target — reflexes and quick directional changes, hitting as many targets as possible before the timer runs out.

User Study
----------

A user study with 15 participants was conducted, the vast majority of whom had no prior drone experience. Each session involved one practice run followed by one official run per controller on a randomly assigned course. Performance metrics (completion time, collision count, DNFs) and perceived workload using the Raw TLX (RTLX) methodology were collected for each run. A Friedman test was used for statistical comparison given the small sample size.

Key Findings
------------

The Pinch controller was the standout result. It achieved the lowest perceived workload across all courses (RTLX of 29.2 versus 41.1 for the PS4), the fastest completion times, the fewest collisions, zero DNFs, and was ranked as the best controller by 93% of participants. Its natural spatial mapping proved to be an interaction paradigm that beginners could learn within a single practice run and perform consistently well with.

The Microgesture controller produced the highest frustration, the worst performance, and a 20% DNF rate. Its impulse-based design was poorly suited for the continuous control that drone piloting demands. The Wrist controller showed moderate results with high physical demand, suggesting potential for experienced users but too steep a learning curve for beginners within a single session.

Technology Stack
----------------

- Unity (URP) — game engine and VR simulation
- Meta Quest 3 — target headset (hand tracking)
- Meta All-In-One-SDK

Repository
----------

Source code for the Bachelor's thesis in Software Engineering, Universidad Complutense de Madrid.

Video Demo
----------
https://youtu.be/lRZlWhpGWs8
