module.exports = class MyVector2 {
    constructor(X = 0, Y = 0) {
        this.x = X;
        this.y = Y;
    }

    Magnitude() {
        return Math.sqrt((this.x * this.x) + (this.y * this.y));
    }

    Normalized() {
        var mag = this.Magnitude();
        return new MyVector2(this.x / mag, this.y / mag);
    }

    Distance(OtherVect = MyVector2) {
        var direction = new MyVector2;
        direction.x = OtherVect.x - this.x;
        direction.y = OtherVect.y - this.y;
        return direction.Magnitude();
    }

    CircleDistance(OtherVect = MyVector2) {
        return (Math.abs(OtherVect.x - this.x) + Math.abs(OtherVect.y - this.y));
    }

    ConsoleOutput() {
        return '(' + this.x + '.' + this.y  + ')';
    }
}