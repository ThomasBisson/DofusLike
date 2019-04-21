Quick Start
I have divided resources into three categories, unit, originunit, effect, and bullet.
I have already matched them in the scene ShowCase, you can choose which special effects and ballistic resources to use. Or you can refer to my use of them.
Note that the original prefab file is under the originalunit directory.
In the Prefab directory you can find prefab with all texture maps and effects.


Scripts
1.ActionEffect.cs -- Action effects playback, automatic control of effects recovery and playback status.
2.ActionEffectManager.cs -- Manage all action effects, call API play and stop effects.
3.BaseEffect.cs -- Special effects base class, which encapsulates some basic interfaces.
4.Bullet.cs -- The bullet base class encapsulates the basic interface of the bullet and is also the basic implementation of the linear linear bullet.
5.CurvelBullet.cs -- Curve trajectory, using the Bezier curve principle to control the ballistic trajectory.
6.FixQueue.cs -- The rendering queue used to modify the material.
7.MathUtil.cs -- General mathematical calculation formula.
8.ParticleScaler.cs -- Used to scale effects.