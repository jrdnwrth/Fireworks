# Ideas

# Standardize particle colors.

Some colors just look more like fireworks.  Lets save these so we can easily reuse them.

## Background Smoke

Have the background scroll slowly from left to right.  As particles pass over it,
have them leave a smudge of smoke.

## Wind

Right now we have a drag variable.  I would rather have a global wind vector
that matches the background scrolling speed.  The drag variable, instead of 
influencing the particle toward zero velocity, would instead influence the particle
toward the wind vector.

## Post processing bloom

Add an extra glow effect, especially where the particles image overlaps smokey parts
of the smoke background image.

## Chaining effects.

I need a way to define an entire firework.  My only idea is to have each effect
expose a callback called OnComplete, which is called when the effect is done.

This function would exist for emitters, particles, etc.  When the emitter's lifespan
gets to zero, then it will pass its position and velocity to this callback (if the function
is not null).

Fireworks can be designed as a chain of effects.  For example, an emitter can fly upward.
 When it reaches the end of its lifespan, it will call its OnComplete function, which will
 create several new emitters at its position to give the burst effect.  When each of those
 emitters completes, they can call their OnComplete function to create a new emitter 
 that does a quick burst of a different color.

## Different size particles.

5 pixels is a nice minimum size.  I would also like size 7 and 9 for larger stars, 
flashes, and strobes.

## Strobes

Add a feature to our particle class that controls the time between flashes, with a
random gaussian distribution.  Similar to how we do lifespan.
