module.exports = class SpellDot {
    constructor() {
        this.timeLeft = 0;
        this.shield = 0;
        this.flatDamageBoost = 0;
        this.additionPA = 0;
        this.additionPM = 0;
        this.damageOverTime = 0;
    }

    PassTime() {
        if (this.timeLeft > 0)
            this.timeLeft -= 1;
        if (this.shield > 0)
            this.shield -= 1;
        if (this.flatDamageBoost > 0)
            this.flatDamageBoost -= 1;
        if (this.additionPA)
            this.additionPA -= 1;
        if (this.additionPM)
            this.additionPM -= 1;
    }

    AddTime(timeLeft) {
        this.timeLeft += timeLeft;
    }

    AddShield(shield) {
        this.shield += shield;
    }

    AddFlatDamageBoost(flatDamageBoost) {
        this.flatDamageBoost += flatDamageBoost;
    }

    AddAdditionnalPA(pa) {
        this.additionPA += pa;
    }

    AddAdditionnalPM(pm) {
        this.additionPM += pm;
    }

    AddDot(dot) {
        this.damageOverTime += dot;
    }
}