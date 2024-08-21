
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSCode : MonoBehaviour
{
    /*
    var Spacewar = (function() {
    "use strict";

    // constants for triangular math (rotation)

    var BIN_RAD_COEF = Math.PI / 51472,  // PI is 51472 (0144420 oct) in PDP-1
        TAU = Math.PI * 2;               // 2 PI

    // game settings

    var Options = {
        ANGULARMOMENTUM:  false,  // sense switch 1
        LOWGRAVITY:       false,  // sense switch 2
        SINGLESHOTS:      false,  // sense switch 3
        NOBACKGROUND:     false,  // sense switch 4
        SUNKILLS:         false,  // sense switch 5
        SUNOFF:           false,  // sense switch 6

        TESTWORDCONTROLS: false,  // use testword controls
        HALTONSCORES:     false,  // halt on scores/matches (call resume() to continue)
        FPS:                 22   // fps (original alternates between 19 and 25)
    };


    // "interesting and often changed constants"
    // (original values in octal, not allowed in JS strict-mode, see comments)

    // sym   value  unit/comment
    // -------------------------------
    var torpedoSupply = 32,  // tno     040  (number of)
        torpedoVelocity = 4,  // tvl  sar 4s  (right shift)
        torpedoReloadTime = 16,  // rlt     020  (frames)
        torpedoLife = 96,  // tlf    0140  (frames)
        fuelSupply = 8192,  // foo  020000  (per exhaust blip)
        angularAcceleration = 8 * BIN_RAD_COEF,  // maa     010  (turn, PI: 0144420)
        spaceshipAcceleration = 4,  // sac  sar 4s  (right shift)
        starCaptureRadius = 1,  // str      01  (greater zero)
        collisionRadius = 48,  // me1   06000  (screen coors)
        collisionRadius2 = 24,  // me2   03000  (above/2)
        torpedoSpaceWarpage = 9,  // the  sar 9s  (right shift)
        hyperspaceShots = 8,  // mhs     010  (number of)
        hyperspaceTimeBeforeBreakout = 32,  // hd1     040  (frames)
        hyperspaceTimeInBreakout = 64,  // hd2    0100  (frames)
        hyperspaceRechargeTime = 128,  // hd3    0200  (frames)
        hyperspaceDisplacement = 9,  // hr1  scl 9s  (left shift)
        hyperspaceInducedVelocity = 4,  // hr2  scl 4s  (left shift)
        hyperspcaceUncertancy = 16384;  // hur  040000  (threshold bonus)

    // ship outline codes

    var outline1 = [           // spaceship 1 (needle)
            1, 1, 1, 1, 3, 1,
            1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 6, 3,
            3, 1, 1, 1, 1, 1,
            1, 4, 6, 1, 1, 1,
            1, 1, 1, 1, 1, 4,
            7, 0, 0, 0, 0, 0
        ],
        outline2 = [           // spaceship 2 (wedge)
            0, 1, 3, 1, 1, 3,
            1, 1, 3, 1, 1, 1,
            1, 1, 6, 3, 1, 3,
            1, 3, 1, 1, 1, 1,
            1, 6, 1, 1, 5, 1,
            1, 1, 1, 6, 3, 3,
            3, 6, 5, 1, 1, 4,
            7, 0, 0, 0, 0, 0
        ];

    /*
      the screen is represented internally like the DEC type 30 display:
      1048 x 1048 at 7 intensities, origin at the center.
      while the original uses a fixed point format (fx10.18) for positions,
      floats are used in the JS implementation.
      intensities are 3 bit values, one's complement (+3 .. -3).
      higher intensities also cause bigger spot-sizes (blips).
      the original display is a point plotted device (animated display),
      meaning there's no memory or frame buffer: blips will be activated
      and fade away (in the afterglow of the Type 30 CRT's P7 phospor).

      co-ordinates are as in the PDP-1 with positive axis up and right:

                 +512

      -511        0/0         +512

                 -511
    */
    /*
    // display constants (for readability)

    var SCREENWIDTH = 1024,
        COORS_MAX = SCREENWIDTH / 2,
        QUADRANT = COORS_MAX / 2;

    // dictionary for controls (exported as property)

    var Controls = {
        SPACESHIP1:  0,
        SPACESHIP2:  1,
        LEFT:        8,
        RIGHT:       4,
        THRUST:      2,
        FIRE:        1,
        HYPERSPACE: 12,  // left + right
        RESET:      15,  // use to clear all
        ALL:        15,
        legalInputs: { 1: true, 2: true, 4: true, 8: true, 12: true, 15: true }
    };

    // static vars

    var mtb = [],        // table of objects
        nob = 24,        // number of objects
        score1 = 0,      // score spaceship 1
        score2 = 0,      // score spaceship 2
        testword = 0,
        timer = 0,
        restartCounter = 0,
        gameCounter = 0,
        halted = false;

    /*
      The testword (var testword) represents the state of an array of switches at
      the operator's console for 18-bit input.
      If Options.TESTWORDCONTROLS is set, input propagates to spaceship controls
      as follows: "high order 4 bits, rotate ccw, rotate cw, (both mean hyperspace)
      fire rocket, and fire torpedo. Low order 4 bits, same for other ship."
      -- player 1: bits 17 .. 14 (left, right, thrust, fire)
      -- player 2: bits  3 .. 0  (left, right, thrust, fire)

      bits 5 .. 10 are related to scoring (bits 6 .. 10: number of games per match)

      bits:         17 16 15 14 13 12 11 10  9  8  7  6  5  4  3  2  1  0
      usage:         -PLAYER 1-          GAMES P. MATCH SC     -PLAYER 2-
      semantics:     L  R  T  F          16  8  4  2  1        L  R  T  F
                     |  |                => 0..31 GAMES        |  |
                  HYPERSPACE                                HYPERSPACE

      SC: Show scores in single game mode or after the next game in match mode.
      (Here, scores will be displayed anyway.) If cleared, when resumed from a
      halt for the score display, scores are reset.
    */

    /*
    // constructors (symbols/pointers of the original program in parentheses)

    function CollidibleObject()
    {
        this.handler = null;       //              (symbol: mtb, pointer: ml1)
        this.collidible = false;   //            (sign-bit in handler address)
        this.x = 0;                // pos x                         (nx1, mx1)
        this.y = 0;                // pos y                         (ny1, my1)
        this.dx = 0;               // delta x                       (ndx, mdx)
        this.dy = 0;               // delta y                       (ndy, mdy)
        this.counter = 0;          // time of torpedo or explosion  (na1, ma1)
        this.size = 0;             // used for explosions           (nb1, mb1)
    }

    function Spaceship()
    {         // extends CollidibleObject
        this.angularMomentum = 0;  //              (symbol: nom, pointer: mom)
        this.theta = 0;            // rotation                      (nth, mth)
        this.fuel = 0;             // amount of fuel                (nfu, mfu)
        this.torpedoes = 0;        // torpedoes left                (ntr, mtr)
        this.outline = null;       // outline code                  (not, mot)
        this.hyp1 = 0;             // hyperspace: handler backup    (nh1, mh1)
        this.hyp2 = 0;             // hyperspace jumps remaining    (nh2, mh2)
        this.hyp3 = 0;             // hyperspace cooling            (nh3, mh3)
        this.hyp4 = 0;             // hyperspace uncertainty        (nh4, mh4)
        this.ctrl = 0;             // control input                 (pntr cwg)
        this.lastCtrl = 0;         // last control word             (nco, mco)
    }
    Spaceship.prototype = new CollidibleObject();


    // main -- start a game

    function run(options )
    {
        // initialize any sense switch flags (optional)
        // e.g., Spacewar.run( { SUNKILLS: true } );
        if (typeof options === 'object')
        {
            for (var k in options) setOption(k, options[k]);
        }

        // start the game
        ExpensivePlanetarium.reset();
        restartCounter = 0;
        frame();
        if (timer) clearInterval(timer);
        timer = setInterval(frame, Math.floor(1000 / Options.FPS));
    }
    */

    #region function newGame()
    /*
    function newGame()
    {  /* (label a40) 
        var i, ss1, ss2;

        // clear and init table of objects (label a2)
        mtb.length = 0;
        for (i = 0; i < 2; i++) mtb.push(new Spaceship());
        for (i = 2; i < nob; i++) mtb.push(new CollidibleObject());

        #region SETUP SPACEHIPS
        // setup spaceships (label a2, a3)
        ss1 = mtb[0];
        ss2 = mtb[1];
        ss1.handler = spaceshipHandler;
        ss1.collidible = true;
        ss2.handler = spaceshipHandler;
        ss2.collidible = true;
        ss1.x = QUADRANT;
        ss1.y = QUADRANT;
        ss2.x = -QUADRANT;
        ss2.y = -QUADRANT;
        ss1.theta = Math.PI;
        ss2.theta = 0;
        ss1.outline = compileOutline(outline1);
        ss2.outline = compileOutline(outline2);
        ss1.torpedoes = ss2.torpedoes = torpedoSupply;
        ss1.fuel = ss2.fuel = fuelSupply;
        ss1.hyp2 = ss2.hyp2 = hyperspaceShots;
        // explosion size will be derived from this (orig: instruction count)
        ss1.size = ss2.size = 1024;
        #endregion
    }
    */
    #endregion

    /*
    function frame()
    {  /* (label a) 
        var ss1, ss2, thriving1, thriving2, endOfMatch = false;
        if (!halted)
        {

            // end of game and restart checks, executed at start of each frame
            // (these were external patches in version 2b)
            if (restartCounter == 0)
            {  /* (label a6) 
                // here after halt (scores display) or at first run
                if ((testword & 32) == 0)
                {
                    // clear scores on testword bit 5 zero
                    score1 = score2 = 0;
                    displayScores(); // notify UI, if found
                    // read new number of games for match play (0 .. 31)
                    gameCounter = (testword >>> 6) & 31;
                }
                newGame();
            }
            // check, if ships are alive and have any torpedoes left
            ss1 = mtb[0];
            ss2 = mtb[1];
            thriving1 = (ss1.handler === spaceshipHandler);
            thriving2 = (ss2.handler === spaceshipHandler);
            if (thriving1 && thriving2
                && (ss1.torpedoes > 0 || ss2.torpedoes > 0))
            {
                // reset restart-counter
                restartCounter = 2 * torpedoLife;
            }
            else if (--restartCounter == 0)
            { // count down to scoring
                // count-down reached: whoever is still alive is awarded a score
                if (thriving1) score1 = (score1 + 1) % 0x3FFFF; // 18 bit overflow
                if (thriving2) score2 = (score2 + 1) % 0x3FFFF;
                displayScores(); // display scores anyway (original see below)
                // check match-play
                if (gameCounter > 0)
                {
                    if (--gameCounter == 0)
                    { // count down
                        // end of a match
                        if (score1 == score2)
                        {
                            // it's a tie, one more game
                            gameCounter = 1;
                        }
                        else
                        {
                            endOfMatch = true;
                        }
                    }
                }
                // halt (to show scores), if match over or bit 5 in testword set,
                // else start over
                if (endOfMatch || (testword & 32))
                {
                    // original puts score 1 into AC and score 2 in IO and halts
                    // displayScores();
                    if (Options.HALTONSCORES)
                    {
                        halted = true;
                        haltSignal(); // notify UI, if found
                    }
                    return; // we'll resume at the top, since restartCounter is still zero
                }
                else
                {
                    // no special case, restart the game
                    newGame();
                    restartCounter = 1; // not in original (fix resume-after-halt logic)
                }
            }

            // finally advance to the main loop ...
            // (in the original program input is read at the start of each spaceship handler)
            // read special input (normally expected to be set by 'Spacewar.setControls()')
            if (Options.TESTWORDCONTROLS)
            {
                // map testword bits to spaceship controls
                mtb[0].ctrl = (testword >> 14) & 15;
                mtb[1].ctrl = testword & 15;
            }
            readGamepads();
            mainLoop();
        }
        CRT.update(); // external UI, not in original
    }
    
    function mainLoop()
    {  /* (label ml0, ml1) 
        var obj1, obj2, nnn = nob - 1;
        // loop over objects
        for (var i = 0; i < nnn; i++)
        {
            obj1 = mtb[i];
            // is it active?
            if (obj1.handler)
            {
                // can it colide?
                if (obj1.collidible)
                {
                    // comparison loop
                    for (var j = i + 1; j < nob; j++)
                    {
                        obj2 = mtb[j];
                        // collidible?
                        if (obj2.collidible)
                        {
                            // evaluate object distances by an octogonal hitbox or proximity
                            var dx = Math.abs(obj1.x - obj2.x);
                            if (dx < collisionRadius)
                            {
                                var dy = Math.abs(obj1.y - obj2.y);
                                if (dy < collisionRadius && dx + dy < collisionRadius2)
                                {
                                    // explode
                                    obj1.handler = obj2.handler = explosionHandler;
                                    obj1.collidible = obj2.collidible = false;
                                    // set up explosion time & size
                                    obj1.counter = obj2.counter = (obj1.size + obj2.size - 1) >> 8;
                                }
                            }
                        }
                    }
                }
                // call the object's method
                obj1.handler();
            }
        }
        // handle last object, if any
        obj1 = mtb[nnn];
        if (obj1.handler) obj1.handler();

        if (!Options.NOBACKGROUND) ExpensivePlanetarium.update(); // background stars
        if (!Options.SUNOFF) drawHeavyStar();    // gravtational star ("sun")
    }

    // object handlers, this-object is current object (see mainLoop)

    function spaceshipHandler()
    {  /* (label sr0) 
        var am, sin, cos, bx, by, t1, t2,
            sx1, sy1, stx, sty, p,
            scn, ssc, ssm, ssn, ssd, csn, csm, scm,
            src, torp, m, f,
            thrusting = false;

        // rotation
        am = this.angularMomentum;
        if (this.ctrl & Controls.LEFT) am += angularAcceleration;
        if (this.ctrl & Controls.RIGHT) am -= angularAcceleration;
        if (Options.ANGULARMOMENTUM)
        {
            this.angularMomentum = am;
        }
        else
        {
            this.angularMomentum = 0;
            am *= 128; // 1<<7
        }
        this.theta += am;
        // limit to +/- 2*PI
        if (this.theta > TAU)
        {
            this.theta -= TAU;
        }
        else if (this.theta < -TAU)
        {
            this.theta += TAU;
        }
        sin = Math.sin(this.theta);
        bx = by = 0;

        // gravity computations
        if (!Options.SUNOFF)
        {
            t1 = this.x / 8;
            t2 = this.y / 8;
            t1 = t1 * t1 + t2 * t2;
            if (t1 < starCaptureRadius)
            { // in sun (label pof)
                this.dx = this.dy = 0;
                if (Options.SUNKILLS)
                { // explode (label po1)
                    this.handler = explosionHandler;
                    this.collidible = false;
                    this.counter = 8;
                }
                else
                { // set ship to "anti pode"
                    this.x = this.y = COORS_MAX;
                }
                return;
            }
            t1 = (Math.sqrt(t1) * t1) / 2;
            if (!Options.LOWGRAVITY) t1 /= 4;
            bx = -this.x / t1;
            by = -this.y / t1;
        }

        // ... and back to business ...
        cos = Math.cos(this.theta);
        // rockets fired?
        if ((this.ctrl & Controls.THRUST) && this.fuel)
        {
            f = 1 << spaceshipAcceleration; // use div instead of right shift
            by += cos / f;
            bx -= sin / f;
            thrusting = true;
        }
        // update positions
        this.dy += by;
        this.y += this.dy / 8;
        this.dx += bx;
        this.x += this.dx / 8;
        toroidalize(this);
        // half a ship's length
        ssn = sin * 16;
        scn = cos * 16;
        // outline start pos (stern, ahead of center)
        sx1 = this.x - ssn;
        sy1 = this.y + scn;
        // torpedoes will show up here
        stx = sx1 - ssn;
        sty = sy1 + scn;
        // draw the ship and update drawing pos to end of outline
        ssn = sin;
        scn = cos;
        ssm = ssn;
        ssc = ssn + scn;
        ssd = ssc;
        csn = ssn - scn;
        csm = -csn;
        scm = scn;
        p = this.outline(sx1, sy1, ssn, scn, ssm, ssc, ssd, csn, csm, scm);
        sx1 = p.x;
        sy1 = p.y;
        // draw exhausts
        if (thrusting)
        {
            src = Math.floor(Math.random() * 16);
            ssn = sin * 2;
            scn = cos * 2;
            // fuel consumption is a function of the blast's length!
            while (this.fuel > 0 && --src > 1)
            {
                this.fuel--;
                sx1 += ssn;
                sy1 -= scn;
                plot(sx1, sy1, 0);
            }
        }
        if (this.counter > 0)
        { // torpedo cooling
            this.counter--;
        }
        else if ( // fire, no single-shot-lock, and torpedoes left?
            (this.ctrl & Controls.FIRE)
            && (!Options.SINGLESHOTS || !(this.lastCtrl & Controls.FIRE))
            && this.torpedoes
        )
        {
            this.torpedoes--;
            // find empty object and set up the torpedo
            for (m = 2; m < nob; m++)
            {
                if (!mtb[m].handler)
                {
                    torp = mtb[m];
                    torp.handler = torpedoHandler;
                    torp.collidible = true;
                    torp.x = stx;
                    torp.y = sty;
                    f = 1 << torpedoVelocity; // use div instead of right shift
                    torp.dx = this.dx - sin * 512 / f;
                    torp.dy = this.dy + cos * 512 / f;
                    torp.size = 16;
                    torp.counter = torpedoLife;
                    this.counter = torpedoReloadTime;
                    break;
                }
            }
        }


        // hyperspace
        if (this.hyp3 > 0)
        { // cooling
            this.hyp3--;
        }
        else if (this.hyp2 > 0)
        { // jumps remaining?
            // are controls for left and right set and was neither of them set before?
            // (last condition is thought to inhibit accidental jumps.
            //  ignored in original, since the last control word is never saved,
            //  works out as "if (this.ctrl) & Controls.HYPERSPACE)".)
            if ((((~this.ctrl) | this.lastCtrl) & Controls.HYPERSPACE) == 0)
            {
                this.hyp1 = this.handler;
                this.handler = hyperspaceHandler;
                this.collidible = false;
                this.counter = hyperspaceTimeBeforeBreakout;
                // this.size = 3; // not used here
            }
        }
        // store last control word (missing in original)
        this.lastCtrl = this.ctrl;
    }
    */


    #region function toroidalise(obj)
    /*
    function toroidalize(obj)
    {
        // util for toroidal space (in original maintained by word length)
        if (obj.x <= -COORS_MAX)
        {
            obj.x += SCREENWIDTH;
        }
        else if (obj.x > COORS_MAX)
        {
            obj.x -= SCREENWIDTH;
        }
        if (obj.y <= -COORS_MAX)
        {
            obj.y += SCREENWIDTH;
        }
        else if (obj.y > COORS_MAX)
        {
            obj.y -= SCREENWIDTH;
        }
    }
    */
    #endregion



    /*
    function explosionHandler()
    {  /* (label mex) 
        var x, y, mxc, f;
        this.y += this.dy / 8;
        this.x += this.dx / 8;
        toroidalize(this);
        // particles
        mxc = this.size >> 3;
        do
        {
            /*
              algorithm:
              1) set up number of right shifts: (mxc > 96)? 1:3
              2) set up number of left shifts: random number int 0..9
              (shifts apply to a combined 36-bit register, 18 bits x and y each,
               sign preserved in x. [x = AC, y = IO])
              3) set x and y to signed 9-bit random numbers
              4) apply right shifts (combined registers)
              5) apply left shifts (combined registers)
              6) add to position and display it
              (only bits 17..9 (hsb-first) are significant for co-ordinates)

              using floats, we apply mult/div instead of shifts:
              mult/div factors are 1 << n.
              any sign flips in y are ignored (just a random number).
            */
    /*
            f = (1 << Math.floor(Math.random() * 9)) / ((mxc > 96) ? 2 : 8);
            x = (Math.random() - 0.5) * 2 * f;
            y = (Math.random() - 0.5) * 2 * f;
            plot(this.x + x, this.y + y, 3);
        } while (--mxc > 0);
        if (--this.counter <= 0) this.handler = null;
    }
    */




    #region function torpedoHandler()
    /*
    function torpedoHandler()
    {  /* (label trc) 
        if (--this.counter < 0)
        {
            // time fuse
            this.counter = 2;
            this.handler = explosionHandler;
            this.collidible = false;
        }
        else
        {  /* (label t1c) 
            this.dy += this.x / (512 * (1 << torpedoSpaceWarpage));
            this.y += this.dy / 8;
            this.dx += this.y / (512 * (1 << torpedoSpaceWarpage));
            this.x += this.dx / 8;
            toroidalize(this);
            plot(this.x, this.y, 1);
        }
    }
    */
    #endregion


    #region function hyperspaceHandler()
    /*
    function hyperspaceHandler()
    {
        // "this routine handles a non-colliding ship invisibly in hyperspace"
        if (--this.counter == 0)
        { // spend time in hyperspace ...
          // zero, set up next step
            this.handler = hyperspaceHandler2;
            // this.size = 7; // not used here
            // set up displacement
            this.x += (Math.random() * 2 - 1) * (1 << hyperspaceDisplacement);
            this.y += (Math.random() * 2 - 1) * (1 << hyperspaceDisplacement);
            toroidalize(this); // maintain toroidal space (not in original)
                               // add induced velocity
            this.dx += (Math.random() * 2 - 1) * (1 << hyperspaceInducedVelocity);
            this.dy += (Math.random() * 2 - 1) * (1 << hyperspaceInducedVelocity);
            // set up a random rotation
            this.theta = TAU * Math.random();
            // original adds some instructions to keep it in bounds of 0 .. 2 PI
            this.counter = hyperspaceTimeInBreakout;
        }
    }
    */
    #endregion


    #region hyperspaceHandler2()
    /*
    function hyperspaceHandler2()
    {
        // "this routine handles a ship breaking out of hyperspace"
        if (--this.counter > 0)
        {
            // spend time in breakout, display a blip
            plot(this.x, this.y, 2);
        }
        else
        {
            // zero, now check, restore spaceship handler
            this.handler = this.hyp1;
            this.collidible = true;
            this.size = 1024;
            if (this.hyp2 > 0) this.hyp2--; // decrement remaining jumps
            this.hyp3 = hyperspaceRechargeTime;
            // now check, if we break on re-entry (Mark One Hyperfield Generators ...)
            this.hyp4 += hyperspcaceUncertancy;
            var r = ((Math.random() * (1 << 20)) | 0) & 0x1FFFF; // 17-bits random
            if (this.hyp4 >= r)
            { // explode
                this.handler = explosionHandler;
                this.collidible = false;
                this.counter = 10;
            }
        }
    }
    */
    #endregion

    /*
    // outline compiler, returns anonymous function
    // variables are rather passed to the resulting function as arguments than shared as globals
    // when called: f(sx1, sy1, ssn, scn, ssm, ssc, ssd, csn, csm, scm)
    // generated function returns object { "x": x, "y": y },
    // where x, y are the coors of the last plot position to be used for sx1 and sy1 respectively.

    function compileOutline(outline)
    { // (label oc)
        var i = 0,
            scan = true,
            max = outline.length,
            oc; // oc: function body of compiled outline code

        function comtab(a, b)
        {
            oc += 'x += ' + a + ';\n' +
                  'y += ' + b + ';\n' +
                  'plot(x, y, 0);\n';
        }

        // compile the function body for plotting the given outline
        // the compiled code represents an unrolled loop in two passes for drawing a side a time
        // code 7 marks the end of the code and initiates the second pass to draw the other side
        // local values passed as arguments:
        //   sx1, sy1: spaceship coors;
        //   ssn, scn, ssm, ssc, ssd, csn, csm, scm: matrix of offsets derived from unit vector

        oc = 'var x, y, stored, xs, ys, t, secondPass = false;\n' +
             'for (;;) {\n' +
             'x = sx1; y = sy1; stored = false;\n';

        while (scan)
        {
            switch (outline[i++])
            {
                case 0: // fall through
                case 1: // advance to rear and plot
                    comtab('ssn', '-scn');
                    break;
                case 2: // advance outwards and plot
                    comtab('scm', 'ssm');
                    break;
                case 3: // advance to rear and outwards and plot
                    comtab('ssc', '-csm');
                    break;
                case 4: // advance inwards and plot
                    comtab('-scm', '-ssm');
                    break;
                case 5: // advance to rear and inwards and plot
                    comtab('csn', '-ssd');
                    break;
                case 6: // store/restore position
                    oc += 'if (stored) { x = xs; y = ys; } else { xs = x; ys = y; }\n' +
                          'stored = !stored;\n';
                    break;
                case 7: // end of code: do mirrored second pass or break out to return
                    oc += 'if (secondPass) break;\n' +
                          'scm = -scm;\n' +
                          'ssm = -ssm;\n' +
                          't = csm;\n' +
                          'csm = ssd\n;' +
                          'ssd = t;\n' +
                          't = ssc;\n' +
                          'ssc = csn;\n' +
                          'csn = t;\n' +
                          'secondPass = true;\n';
                    scan = false;
                    break;
            }
            if (scan && i === max)
            { // warn and fix corrupted outline (not in original)
                console.warn('Outline Compiler: Outline without code 7!');
                outline.push(7);
            }
        }

        // code to close the loop executing a pass and to return the last plotting position
        oc += '}\n' +
                 'return { "x": x, "y": y };\n';

        // return the generated function
        return eval('(function(sx1, sy1, ssn, scn, ssm, ssc, ssd, csn, csm, scm) {' + oc + '});');
    }

    /*
    // alternate outline interpreter, takes array of outline codes as the first argument

        function drawOutline(oc, sx1, sy1, ssn, scn, ssm, ssc, ssd, csn, csm, scm) {
            // (lable oc => properties 'not', 'not+1')
            var pass1 = true,
                mark = false,
                x = sx1,
                y = sx2,
                i = 0,
                max = oc.length,
                mx, my, t;

            while (i < max) {
                var x1=x, y1=y;
                switch (oc[i++]) {
                    case 0: // fall through
                    case 1:    // down
                        x += ssn;
                        y -= scn;
                        plot(x, y, 0);
                        break;
                    case 2:  // right
                        x += scm;
                        y += ssm;
                        plot(x, y, 0);
                        break;
                    case 3:    // down right
                        x += ssc;
                        y -= csm;
                        plot(x, y, 0);
                        break;
                    case 4: // left
                        x -= scm;
                        y -= ssm;
                        plot(x, y, 0);
                        break;
                    case 5:    // down left
                        x += csn;
                        y -= ssd;
                        plot(x, y, 0);
                        break;
                    case 6: // store/restore position
                        if (mark) {
                            x = mx;
                            y = my;
                        }
                        else {
                            mx = x;
                            my = y;
                        }
                        mark = !mark;
                        break;
                    case 7: // mirror / return
                        if (pass1) {
                            // flip matrix horizontally
                            scm = -scm;
                            ssm = -ssm;
                            t = csm;
                            csm = ssd;
                            ssd = t;
                            t = ssc;
                            ssc = csn;
                            csn = t;
                            // start over
                            i = 0;
                            x = sx1;
                            y = sx2;
                            pass1 = false;
                            mark = false; // fix any orphaned codes 6 (not in original)
                        }
                        else {
                            // return last plotting position
                            return {
                                'x': x,
                                'y': y
                            };
                        }
                        break;
                }
            }
            // failsafe return -- not in original, we should have returned at code 7 before!
            return {
                'x': x,
                'y': y
            };
        }
    */
    /*
    // draw the gravitational star

    function drawHeavyStar()
    { // (label blp)
        var x = 0,
            y = 0,
            bx = Math.random() * 2 - 1,
            by = Math.random() * 2 - 1,
            l = 16 - Math.floor(Math.random() * 8),
            i;

        function starp()
        {
            x += bx;
            y += by;
            plot(x, y, 0);
        }

        plot(x, y, 0);
        for (i = 0; i < l; i++) starp();
        x = -x;
        y = -y;
        for (i = 0; i < l; i++) starp();
    }
    */

    #region PLANATARIUM
    /*
    // the program in the program ...

    var ExpensivePlanetarium = (function() {
        "use strict";

    // "background display 3/13/62, prs."

    // config
    var NOSKIP = false,   // opt out of skip of every 2nd frame
        INTERLACED = false;   // Spacewar 2b - mode

    // static vars
    var m1,            // Magnitude('1j') (defined below the star data)
        m2,            // Magnitude('2j')
        m3,            // Magnitude('3j')
        m4,            // Magnitude('4j')
        bcc = 0,       // frame skip counter (spacewar 3.1 mode)
        bkc = 0,       // step counter
        fpr = 4096;    // (010000) right frame margin, map center

    // dimensional constants (for readability)
    var SCREEN_WIDTH = 1024,
        HALFSCREEN = SCREEN_WIDTH / 2,
        MAP_WIDTH = 8192;  // range of x-domain (0 .. 020000)

    // constructor, in original an assembler macro

    function Magnitude(label)
    {
        var data = stars[label];
        this.coors = [];  // star data
        this.cursor = 0;  // data cursor (label flo, initialized to offset J)

        // transform coors (like macro "mark")
        for (var i = 0; i < data.length; i += 2)
        {
            // x-coor: max - x (flip horizontally)
            this.coors.push(MAP_WIDTH - data[i]);
            // y-coor here as is (orig. prepares for display instructions)
            this.coors.push(data[i + 1]);
        }
        data = null;
    }

    Magnitude.prototype.dislis = function(b)
    { /* (macro dislis) 
        // scans stars, displays on-screen ones,
        // limits scan per frame by cursor-pos. (stars are ordered by x-coors)
        // the original implementation loops over a subset of the entire set
        // of coordinates, in bounds of offsets J and Q+2 (Q: index of last x)
        // here, coors are private, J is always zero and Q+2 is coors.length.
        // argument b: brightness (display intensity).

        var didPlot = false,         // (program flag 5)
            startIdx = this.cursor,  // data index on entry (label fpo)
            cx = this.cursor,        // data pos x-coor (label fin)
            cy = this.cursor + 1,    // data pos y-coor (label fyn)
            sx;                      // screen x-coor (just AC in original)

        for (; ; )
        { // loop over coors (label fin)
          // prepare the screen x-coordinate for display
            sx = this.coors[cx] - fpr; // x-coor - right margin
                                       // check, if inbounds to the right
            if (sx >= 0)
            { // off-screen, try wrap/overlap (label fgr)
                sx = -MAP_WIDTH + SCREEN_WIDTH;
            }
            else
            {
                sx += SCREEN_WIDTH;
            }
            // check, if inbounds to the left (label frr, fou)
            if (sx <= 0)
            { // off-screen to the left (label fuu)
              // did we plot any?
                if (didPlot) break;
                // since the view moves from right to left, we won't inspect it in
                // the next run again, advance the data cursor (start with next star)
                this.cursor += 2;
                // the original loops over offsets J .. Q+2
                if (this.cursor == this.coors.length) this.cursor = 0;
            }
            else
            { // on-screen (0 >= sx > SCREEN_WIDTH)
              // display it (label fie)
                plot(sx - HALFSCREEN, this.coors[cy], b); // in main
                didPlot = true;
            }
            // next star (label fid)
            cy++;
            if (cy == this.coors.length)
            { // data wrap-around (label flp; length: Q+2)
              // did we start at 0 (have we seen all)?
                if (startIdx == 0) break; // in original offset J, here always zero
                                          // no, start over with first star
                cx = 0;  // in original offset J, here always zero
                cy = 1;  // in original J+1
            }
            else
            {
                // have we performed a full wrap (seen all)?
                if (cy == startIdx) break;
                // continue with next star (next pair of coors)
                cx = cy;
                cy++;
            }
        }
    };

    function update()
    { /* main method ( label bck ) 
        // two implementations: once as in sw 3.1 and once as in sw 2b
        if (!INTERLACED)
        {
            // spacewar 3.1: stars modulated by display intensities
            // displays every second frame only (opt-out option not in original code)
            if (!NOSKIP)
            {
                if (++bcc < 0) return;
                bcc = -2;
            }
            m1.dislis(3);
            m2.dislis(2);
            m3.dislis(1);
            m4.dislis(0);
            // advance every 32nd frame (every 16th display frame)
            if (++bkc >= 0)
            {
                bkc = (NOSKIP) ? -32 : -16;
                fpr--;
                if (fpr < 0) fpr += MAP_WIDTH; // reset right margin (8192)
            }
        }
        else
        {
            // spacewar 2b: stars modulated by frequency of update
            // (requires emulation of P7 phosphor, speed options not implemented)
            m1.dislis(0);
            bcc++;
            if (bcc & 1) m3.dislis(0);
            m2.dislis(0);
            if (bcc & 3 != 0) m4.dislis(0);
            m1.dislis(0); // display first magnitude again (brightest)
            bcc %= 4;     // not in the original code (wraps around by overflow)
                          // advance every 32nd frame
            if (++bkc >= 0)
            {
                bkc = -32;
                fpr--;
                if (fpr < 0) fpr += MAP_WIDTH; // reset right margin
            }
        }
    }

    // API methods (exported)

    function setOption(key, v)
    {
        var k = String(key).toUpperCase().replace(/[^A - Z0 - 9] / g, '');
        switch (k)
        {
            case 'FRAMESKIP':
                v = !v; // fall through
            case 'NOSKIP':
                if (NOSKIP && bkc < -16) bkc += 16;
                NOSKIP = Boolean(v);
                break;
            case 'INTERLACED':
            case 'SPACEWAR2B':
                INTERLACED = Boolean(v);
                bcc = bkc = 0;
                break;
        }
    }

    function reset()
    {
        fpr = 4096;
        bcc = bkc = 0;
    }

    /*
      star data: "stars by prs 3/13/62 for s/w 2b"

      stars of 1st to 4th magnitude (1j .. 4j) of segment 22.5 deg N to 22.5 deg S
      data order: x, y (ascending by x), x-range: 0 .. 8192, y-range: -511 .. +512
      units: screen coors, x: offset from right margin, y: N (top) < 0 < S (bottom)
      (y-domain scaled to internal screen representation, x-domain proportional.)
    */
    /*
    var stars = {
            '1j': [
                1537,  371, //  87 Taur, Aldebaran
                1762, -189, //  19 Orio, Rigel
                1990,  168, //  58 Orio, Betelgeuze
                2280, -377, //   9 CMaj, Sirius
                2583,  125, //  10 CMin, Procyon
                3431,  283, //  32 Leon, Regulus
                4551, -242, //  67 Virg, Spica
                4842,  448, //  16 Boot, Arcturus
                6747,  196  //  53 Aqil, Altair
            ],
            '2j': [
                1819,  143, //  24 Orio, Bellatrix
                1884,  -29, //  46 Orio
                1910,  -46, //  50 Orio
                1951, -221, //  53 Orio
                2152, -407, //   2 CMaj
                2230,  375, //  24 Gemi
                3201, -187, //  30 Hyda, Alphard
                4005,  344, //  94 Leon, Denebola
                5975,  288  //  55 Ophi
            ],
            '3j': [
                  46,  333, //  88 Pegs, Algenib
                 362, -244, //  31 Ceti
                 490,  338, //  99 Pisc
                 566, -375, //  52 Ceti
                 621,  462, //   6 Arie
                 764,  -78, //  68 Ceti, Mira
                 900,   64, //  86 Ceti
                1007,   84, //  92 Ceti
                1243, -230, //  23 Erid
                1328, -314, //  34 Erid
                1495,  432, //  74 Taur
                1496,  356, //  78 Taur
                1618,  154, //   1 Orio
                1644,   52, //   8 Orio
                1723, -119, //  67 Erid
                1755, -371, //   5 Leps
                1779, -158, //  20 Orio
                1817,  -57, //  28 Orio
                1843, -474, //   9 Leps
                1860,   -8, //  34 Orio
                1868, -407, //  11 Leps
                1875,  225, //  39 Orio
                1880, -136, //  44 Orio
                1887,  480, // 123 Taur
                1948, -338, //  14 Leps
                2274,  296, //  31 Gemi
                2460,  380, //  54 Gemi
                2470,  504, //  55 Gemi
                2513,  193, //   3 CMin
                2967,  154, //  11 Hyda
                3016,  144, //  16 Hyda
                3424,  393, //  30 Leon
                3496,  463, //  41 Leon, Algieba
                3668, -357, //  nu Hyda
                3805,  479, //  68 Leon
                3806,  364, //  10 Leon
                4124, -502, //   2 Corv
                4157, -387, //   4 Corv
                4236, -363, //   7 Corv
                4304,  -21, //  29 Virg
                4384,  90,  //  43 Virg
                4421,  262, //  47 Virg
                4606,   -2, //  79 Virg
                4721,  430, //   8 Boot
                5037, -356, //   9 Libr
                5186, -205, //  27 Libr
                5344,  153, //  24 Serp
                5357,  358, //  28 Serp
                5373,  -71, //  32 Serp
                5430, -508, //   7 Scor
                5459, -445, //   8 Scor
                5513,  -78, //   1 Ophi
                5536, -101, //   2 Ophi
                5609,  494, //  27 Herc
                5641, -236, //  13 Ophi
                5828, -355, //  35 Ophi
                5860,  330, //  64 Herc
                5984, -349, //  55 Serp
                6047,   63, //  62 Ophi
                6107, -222, //  64 Ophi
                6159,  217, //  72 Ophi
                6236,  -66, //  58 Serp
                6439, -483, //  37 Sgtr
                6490,  312, //  17 Aqil
                6491, -115, //  16 Aqil
                6507, -482, //  41 Sgtr
                6602,   66, //  30 Aqil
                6721,  236, //  50 Aqil
                6794,  437, //  12 Sgte
                6862,  -25, //  65 Aqil
                6914, -344, //   9 Capr
                7014,  324, //   6 Dlph
                7318, -137, //  22 Aqar
                7391,  214, //   8 Pegs
                7404, -377, //  49 Capr
                7513,  -18, //  34 Aqar
                7539,  130, //  26 Pegs
                7644,  -12, //  55 Aqar
                7717,  235, //  42 Pegs
                7790, -372, //  76 Aqar
                7849,  334  //  54 Pegs, Markab
            ],
            '4j': [
                   1, -143, //  33 Pisc
                  54,  447, //  89 Pegs
                  54, -443, //   7 Ceti
                  82, -214, //   8 Ceti
                 223, -254, //  17 Ceti
                 248,  160, //  63 Pisc
                 273,  -38, //  20 Ceti
                 329,  167, //  71 Pisc
                 376,  467, //  84 Pisc
                 450, -198, //  45 Ceti
                 548,  113, // 106 Pisc
                 570,  197, // 110 Pisc
                 595, -255, //  53 Ceti
                 606, -247, //  55 Ceti
                 615,  428, //   5 Arie
                 617,   61, //  14 Pisc
                 656, -491, //  59 Ceti
                 665,   52, // 113 Pisc
                 727,  191, //  65 Ceti
                 803, -290, //  72 Ceti
                 813,  182, //  73 Ceti
                 838, -357, //  76 Ceti
                 878,   -2, //  82 Ceti
                 907, -340, //  89 Ceti
                 908,  221, //  87 Ceti
                 913, -432, //   1 Erid
                 947, -487, //   2 Erid
                 976, -212, //   3 Erid
                 992,  194, //  91 Ceti
                1058,  440, //  57 Arie
                1076,  470, //  58 Arie
                1087, -209, //  13 Erid
                1104,   68, //  96 Ceti
                1110, -503, //  16 Erid
                1135,  198, //   1 Taur
                1148,  214, //   2 Taur
                1168,  287, //   5 Taur
                1170, -123, //  17 Erid
                1185, -223, //  18 Erid
                1191, -500, //  19 Erid
                1205,    2, //  10 Taur
                1260, -283, //  26 Erid
                1304,  -74, //  32 Erid
                1338,  278, //  35 Taur
                1353,  130, //  38 Taur
                1358,  497, //  37 Taur
                1405, -162, //  38 Erid
                1414,  205, //  47 Taur
                1423,  197, //  49 Taur
                1426, -178, //  40 Erid
                1430,  463, //  50 Taur
                1446,  350, //  54 Taur
                1463,  394, //  61 Taur
                1470,  392, //  64 Taur
                1476,  502, //  65 Taur
                1477,  403, //  68 Taur
                1483,  350, //  71 Taur
                1485,  330, //  73 Taur
                1495,  358, //  77 Taur
                1507,  364, //
                1518,   -6, //  45 Erid
                1526,  333, //  86 Taur
                1537,  226, //  88 Taur
                1544,  -81, //  48 Erid
                1551,  280, //  90 Taur
                1556,  358, //  92 Taur
                1557, -330, //  53 Erid
                1571, -452, //  54 Erid
                1596,  -78, //  57 Erid
                1622,  199, //   2 Orio
                1626,  124, //   3 Orio
                1638, -128, //  61 Erid
                1646,  228, //   7 Orio
                1654,  304, //   9 Orio
                1669,   36, //  10 Orio
                1680, -289, //  64 Erid
                1687, -167, //  65 Erid
                1690, -460, //
                1690,  488, // 102 Taur
                1700,  347, //  11 Orio
                1729,  352, //  15 Orio
                1732, -202, //  69 Erid
                1750, -273, //   3 Leps
                1753,   63, //  17 Orio
                1756, -297, //   4 Leps
                1792, -302, //   6 Leps
                1799, -486, //
                1801,  -11, //  22 Orio
                1807,   79, //  23 Orio
                1816, -180, //  29 Orio
                1818,   40, //  25 Orio
                1830,  497, // 114 Taur
                1830,   69, //  30 Orio
                1851,  134, //  32 Orio
                1857,  421, // 119 Taur
                1861, -168, //  36 Orio
                1874,  214, //  37 Orio
                1878, -138, //
                1880, -112, //  42 Orio
                1885,  210, //  40 Orio
                1899,  -60, //  48 Orio
                1900,   93, //  47 Orio
                1900, -165, //  49 Orio
                1909,  375, // 126 Taur
                1936, -511, //  13 Leps
                1957, 287,  // 134 Taur
                1974, -475, //  15 Leps
                1982,  461, //  54 Orio
                2002, -323, //  16 Leps
                2020,  -70, //
                2030,  220, //  61 Orio
                2032, -241, //   3 Mono
                2037,  458, //  62 Orio
                2057, -340, //  18 Leps
                2059,  336, //  67 Orio
                2084,  368, //  69 Orio
                2084,  324, //  70 Orio
                2105, -142, //   5 Mono
                2112, -311, //
                2153,  106, //   8 Mono
                2179,  462, //  18 Gemi
                2179, -107, //  10 Mono
                2184, -159, //  11 Mono
                2204,  168, //  13 Mono
                2232, -436, //   7 CMaj
                2239, -413, //   8 CMaj
                2245, -320, //
                2250,  227, //  15 Mono
                2266,  303, //  30 Gemi
                2291,   57, //  18 Mono
                2327,  303, //  38 Gemi
                2328, -457, //  15 CMaj
                2330, -271, //  14 CMaj
                2340, -456, //  19 CMaj
                2342, -385, //  20 CMaj
                2378,  -93, //  19 Mono
                2379,  471, //  43 Gemi
                2385, -352, //  23 CMaj
                2428,   -8, //  22 Mono
                2491, -429, //
                2519,  208, //   4 CMin
                2527,  278, //   6 CMin
                2559, -503, //
                2597, -212, //  26 Mono
                2704, -412, //
                2709,  -25, //  28 Mono
                2714,   60, //
                2751,  -61, //  29 Mono
                2757, -431, //  16 Pupp
                2768, -288, //  19 Pupp
                2794,  216, //  17 Canc
                2848,  -82, //
                2915,  138, //   4 Hyda
                2921,   84, //   5 Hyda
                2942, -355, //   9 Hyda
                2944,  497, //  43 Canc
                2947,   85, //   7 Hyda
                2951, -156, //
                2953,  421, //  47 Canc
                2968, -300, //  12 Hyda
                2976,  141, //  13 Hyda
                3032,  279, //  65 Canc
                3124,   62, //  22 Hyda
                3157, -263, //  26 Hyda
                3161, -208, //  27 Hyda
                3209,  -53, //  31 Hyda
                3225,  -17, //  32 Hyda
                3261,  116, //
                3270,  -16, //  35 Hyda
                3274, -316, //  38 Hyda
                3276,  236, //  14 Leon
                3338, -327, //  39 Hyda
                3385,  194, //  29 Leon
                3415, -286, //  40 Hyda
                3428,  239, //  31 Leon
                3429,    3, //  15 Sext
                3446, -270, //  41 Hyda
                3495,  455, //  40 Leon
                3534, -372, //  42 Hyda
                3557,   -3, //  30 Sext
                3570,  223, //  47 Leon
                3726, -404, //  al Crat
                3736,  -44, //  61 Leon
                3738,  471, //  60 Leon
                3754,  179, //  63 Leon
                3793, -507, //  11 Crat
                3821,  -71, //  74 Leon
                3836, -324, //  12 Crat
                3846,  150, //  77 Leon
                3861,  252, //  78 Leon
                3868, -390, //  15 Crat
                3935, -211, //  21 Crat
                3936,   -6, //  91 Leon
                3981, -405, //  27 Crat
                3986,  161, //   3 Virg
                3998,  473, //  93 Leon
                4013,   53, //   5 Virg
                4072,  163, //   8 Virg
                4097,  211, //   9 Virg
                4180,   -3, //  15 Virg
                4185,  418, //  11 Coma
                4249, -356, //   8 Corv
                4290, -170, //  26 Virg
                4305,  245, //  30 Virg
                4376, -205, //  40 Virg
                4403,  409, //  36 Coma
                4465, -114, //  51 Virg
                4466,  411, //  42 Coma
                4512, -404, //  61 Virg
                4563, -352, //  69 Virg
                4590, -131, //  74 Virg
                4603,   95, //  78 Virg
                4679,  409, //   4 Boot
                4691,  371, //   5 Boot
                4759,   46, //  93 Virg
                4820,   66, //
                4822, -223, //  98 Virg
                4840, -126, //  99 Virg
                4857, -294, // 100 Virg
                4864,  382, //  20 Boot
                4910,  -41, // 105 Virg
                4984,  383, //  29 Boot
                4986,  322, //  30 Boot
                4994, -119, // 107 Virg
                5009,  396, //  35 Boot
                5013,   53, // 109 Virg
                5045,  444, //  37 Boot
                5074,  -90, //  16 Libr
                5108,   57, // 110 Virg
                5157, -442, //  24 Libr
                5283, -221, //  37 Libr
                5290, -329, //  38 Libr
                5291,  247, //  13 Serp
                5326, -440, //  43 Libr
                5331,  455, //  21 Serp
                5357,  175, //  27 Serp
                5372,  420, //  35 Serp
                5381,  109, //  37 Serp
                5387,  484, //  38 Serp
                5394, -374, //  46 Libr
                5415,  364, //  41 Serp
                5419, -318, //  48 Libr
                5455, -253, //  xi Scor
                5467, -464, //   9 Scor
                5470, -469, //  10 Scor
                5497, -437, //  14 Scor
                5499, -223, //  15 Scor
                5558,   29, //  50 Serp
                5561,  441, //  20 Herc
                5565, -451, //   4 Ophi
                5580,  325, //  24 Herc
                5582, -415, //   7 Ophi
                5589, -186, //   3 Ophi
                5606, -373, //   8 Ophi
                5609,   50, //  10 Ophi
                5610, -484, //   9 Ophi
                5620,  266, //  29 Herc
                5713, -241, //  20 Ophi
                5742,  235, //  25 Ophi
                5763,  217, //  27 Ophi
                5807,  293, //  60 Herc
                5868,   -8, //  41 Ophi
                5888, -478, //  40 Ophi
                5889, -290, //  53 Serp
                5924, -114, //
                5925,   96, //  49 Ophi
                5987, -183, //  57 Ophi
                6006, -292, //  56 Serp
                6016, -492, //  58 Ophi
                6117,  -84, //  57 Serp
                6117,   99, //  66 Ophi
                6119,  381, //  93 Herc
                6119,   67, //  67 Ophi
                6125,   30, //  68 Ophi
                6146,   57, //  70 Ophi
                6158,  198, //  71 Ophi
                6170,  473, // 102 Herc
                6188, -480, //  13 Sgtr
                6234,   76, //  74 Ophi
                6235,  499, // 106 Herc
                6247, -204, //  xi Scut
                6254, -469, //  21 Sgtr
                6255,  494, // 109 Herc
                6278, -333, //  ga Scut
                6313, -189, //  al Scut
                6379,  465, // 110 Herc
                6382, -110, //  be Scut
                6386,  411, // 111 Herc
                6436,   93, //  63 Serp
                6457,  340, //  13 Aqil
                6465, -134, //  12 Aqil
                6478, -498, //  39 Sgtr
                6553,  483, //   1 Vulp
                6576, -410, //  44 Sgtr
                6576, -368, //  46 Sgtr
                6607,    3, //  32 Aqil
                6651,  163, //  38 Aqil
                6657,  445, //   9 Vulp
                6665,  -35, //  41 Aqil
                6688,  405, //   5 Sgte
                6693,  393, //   6 Sgte
                6730,  416, //   7 Sgte
                6739,  430, //   8 Sgte
                6755,   17, //  55 Aqil
                6766,  187, //  59 Aqil
                6772,  140, //  60 Aqil
                6882,  339, //  67 Aqil
                6896, -292, //   5 Capr
                6898, -292, //   6 Capr
                6913, -297, //   8 Capr
                6958, -413, //  11 Capr
                6988,  250, //   2 Dlph
                7001,  326, //   4 Dlph
                7015,  -33, //  71 Aqil
                7020,  475, //  29 Vulp
                7026,  354, //   9 Dlph
                7047,  335, //  11 Dlph
                7066,  359, //  12 Dlph
                7067, -225, //   2 Aqar
                7068, -123, //   3 Aqar
                7096, -213, //   6 Aqar
                7161, -461, //  22 Capr
                7170, -401, //  23 Capr
                7192, -268, //  13 Aqar
                7199,  222, //   5 Equl
                7223,  219, //   7 Equl
                7230,  110, //   8 Equl
                7263, -393, //  32 Capr
                7267,  441, //   1 Pegs
                7299, -506, //  36 Capr
                7347, -453, //  39 Capr
                7353, -189, //  23 Aqar
                7365, -390, //  40 Capr
                7379, -440, //  43 Capr
                7394,  384, //   9 Pegs
                7499,  -60, //  31 Aqar
                7513,  104, //  22 Pegs
                7515, -327, //  33 Aqar
                7575, -189, //  43 Aqar
                7603,  -43, //  48 Aqar
                7604,  266, //  31 Pegs
                7624,   20, //  52 Aqar
                7639,   96, //  35 Pegs
                7654, -255, //  57 Aqar
                7681,  -14, //  62 Aqar
                7727, -440, //  66 Aqar
                7747,  266, //  46 Pegs
                7761, -321, //  71 Aqar
                7779, -185, //  73 Aqar
                7795,  189, //  50 Pegs
                7844,   75, //   4 Pisc
                7862,  202, //  55 Pegs
                7874, -494, //  88 Aqar
                7903, -150, //  90 Aqar
                7911, -219, //  91 Aqar
                7919,   62, //   6 Pisc
                7923, -222, //  93 Aqar
                7952, -470, //  98 Aqar
                7969, -482, //  99 Aqar
                7975,   16, //   8 Pisc
                7981,  133, //  10 Pisc
                7988,  278, //  70 Pegs
                8010, -489, // 101 Aqar
                8049,  116, //  17 Pisc
                8059, -418, // 104 Aqar
                8061,   28, //  18 Pisc
                8064, -344, // 105 Aqar
                8159,  144, //  28 Pisc
                8174, -149, //  30 Pisc
                8188, -407  //   2 Ceti
            ]
};

m1 = new Magnitude('1j');
m2 = new Magnitude('2j');
m3 = new Magnitude('3j');
m4 = new Magnitude('4j');

// return API

return {
    'update': update,
            'setOption': setOption,
            'reset': reset
        };
    })();
    */
    #endregion


    /* ====== implementation specific glue ====== */

    // plot int co-ordinates, normalized (x: 0..1024, y: 0..1024, origin top-left) to CRT
    /*
    function plot(x, y, b)
    {
        x = (COORS_MAX + x) % SCREENWIDTH;
        y = (SCREENWIDTH - (COORS_MAX + y)) % SCREENWIDTH;
        if (x < 0) x += SCREENWIDTH;
        if (y < 0) y += SCREENWIDTH;
        CRT.plot(x, y, b);
        /* alternatively plot to integer coors:
          CRT.plot(x|0, y|0, b);
        */
    /*
    }

    // optional UI notifications

    function displayScores()
    {
        if ((typeof SpacewarUI === 'object' || typeof SpacewarUI === 'function')
            && typeof SpacewarUI.showScores === 'function')
        {
            SpacewarUI.showScores(score1, score2);
        }
    }

    function haltSignal()
    {
        if ((typeof SpacewarUI === 'object' || typeof SpacewarUI === 'function')
             && typeof SpacewarUI.halted === 'function')
        {
            SpacewarUI.halted();
        }
    }

    // notify any external UI to check gamepads (on the entry of each frame).
    // the UI is expected to set any readings via Spacewar.setControls().

    function readGamepads()
    {
        if ((typeof SpacewarUI === 'object' || typeof SpacewarUI === 'function')
            && typeof SpacewarUI.readGamepads === 'function')
        {
            SpacewarUI.readGamepads();
        }
    }

    // some sane setters for external use

    /*
      setter for sense switch settings
      e.g., Spacewar.setOption( 'ANGULARMOMENTUM', true )
      or    Spacewar.setOption( 'SenseSwitch 1', true )
    */
    /*
    function setOption(key, value)
    {
        var k = String(key).toUpperCase().replace(/[^A - Z1 - 6] / g, '');
        switch (k)
        {
            case 'SENSESWITCH1': Options.ANGULARMOMENTUM = Boolean(value); break;
            case 'SENSESWITCH2': Options.LOWGRAVITY = Boolean(value); break;
            case 'SENSESWITCH3': Options.SINGLESHOTS = Boolean(value); break;
            case 'SENSESWITCH4': Options.NOBACKGROUND = Boolean(value); break;
            case 'SENSESWITCH5': Options.SUNKILLS = Boolean(value); break;
            case 'SENSESWITCH6': Options.SUNOFF = Boolean(value); break;
            case 'FPS': setFPS(value); break;
            default:
                if (typeof Options[k] !== 'undefined') Options[k] = Boolean(value);
                break;
        }
    }

    /*
      getter for Options
    */
    /*
    function getOption(key)
    {
        var k = String(key).toUpperCase().replace(/[^A - Z1 - 6] / g, '');
        switch (k)
        {
            case 'SENSESWITCH1': return Options.ANGULARMOMENTUM;
            case 'SENSESWITCH2': return Options.LOWGRAVITY;
            case 'SENSESWITCH3': return Options.SINGLESHOTS;
            case 'SENSESWITCH4': return Options.NOBACKGROUND;
            case 'SENSESWITCH5': return Options.SUNKILLS;
            case 'SENSESWITCH6': return Options.SUNOFF;
            case 'FPS': return FPS;
            default: return Options[k];
        }
    }

    /*
      setter for spaceship control input
      e.g., Spacewar.setControls( 'SPACESHIP1', 'FIRE', true )
      or    Spacewar.setControls( 0, 'FIRE', true )
      or    Spacewar.setControls( Spacewar.Controls.SPACESHIP1, Spacewar.Controls.FIRE, true )
      reset state by Spacewar.setControls( 'SPACESHIP1', 'RESET' )
      or    Spacewar.setControls( 'SPACESHIP1', 'ALL', false )
    */
    /*
    function setControls(spaceship, key, value)
    {
        var s, b, obj;
        if (!mtb.length) return; // not started, yet: ignore
                                 // sanitize input
        switch (typeof spaceship) {
            case 'number':
                s = spaceship | 0;
                if (s < 0 || s > 1) return;
                break;
            case 'string':
                spaceship = spaceship.toUpperCase();
                if (spaceship === 'SPACESHIP1' || spaceship === 'SPACESHIP2')
                {
                    s = Controls[spaceship];
                    break;
                }
                else
                {
                    return;
                }
            default: return;
        }
        switch (typeof key) {
            case 'number':
                b = key | 0;
                if (!Controls.legalInputs[b]) return;
                break;
            case 'string':
                key = key.toUpperCase();
                if (Controls[key] !== 'undefined')
                {
                    b = Controls[key];
                    break;
                }
                else
                {
                    return;
                }
            default: return;
        }
        // finally, manipulate the bit-vector in property 'ctrl'
        obj = mtb[s];
        obj.ctrl = ((value) ? obj.ctrl | b : obj.ctrl & (~b)) & Controls.ALL;
    }

    /*
      setter for testword
    */
    /*
    function setTestword(n)
    {
        var tw = parseInt(String(n), 10);
        if (!isNaN(tw)) testword = Math.abs(tw) & 0x3FFFF;
    }

    /*
        getter for testword
    */
    /*
    function getTestword()
    {
        return testword;
    }

    /*
      setter for FPS, called by "setOption('FPF', <value>)" internally
    */
    /*
    function setFPS(n)
    {
        var fps = parseFloat(String(n));
        if (!isNaN(fps) && fps > 0 && fps <= 100)
        {
            Options.FPS = fps;
            if (timer)
            {
                clearInterval(timer);
                timer = setInterval(frame, Math.floor(1000 / fps));
            }
        }
    }

    /*
        getter for FPS
    */
    /*
    function getFPS()
    {
        return FPS;
    }

    /*
        getter for scores, returns array [<score-ss1>, <score-ss2>]
    */
    /*
    function getScores()
    {
        return [score1, score2];
    }

    /*
        clear/reset scores
    */
    /*
    function resetScores()
    {
        score1 = score2 = 0;
        displayScores();
    }

    /*
        halt execution
    */
    /*
    function halt()
    {
        if (timer) halted = true;
    }

    /*
        resume from halt
    */
    /*:
    function resume()
    {
        halted = false;
    }

    // return API object

    return {
        'setOption': setOption,
            'Controls': Controls,
            'setControls': setControls,
            'setTestword': setTestword,
            'getTestword': getTestword,
            'setFPS': setFPS,
            'getFPS': getFPS,
            'getScores': getScores,
            'resetScores': resetScores,
            'halt': halt,
            'resume': resume,
            'run': run,

            'EPsetOption': ExpensivePlanetarium.setOption
        };

    })();
    */

}
