
namespace SerializableClass
{

    [System.Serializable]
    public class Player
    {
        public string id;
        public string username;
        public Characteristic characteristic;
        public MyVector3 positionInWorld;
        public MyVector2 positionArrayMain;
        public MyVector2 positionArrayFight;

        public Player()
        {
            positionInWorld = new MyVector3();
            positionArrayMain = new MyVector2();
            positionArrayFight = new MyVector2();
        }
    }

    [System.Serializable]
    public class Characteristic
    {
        public string name;
        public int baseHealthPoints;
        public int baseActionPoints;
        public int baseMovementPoints;
        public string[] spells;
        public Spell[] myspells;
}
}