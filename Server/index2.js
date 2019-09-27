var app = require('express')();
var server = require('http').Server(app);
var io = require('socket.io')(server);

const fs = require('fs');
const readline = require('readline');

//const MongoDB2 = require("MyMongoDB2")

server.listen(60000, function() {

    console.log("Server is now running...");

    (async function () {
        var db = new MyMongoDB2();
        await db.connection();

        console.log("Connected to database...");

        var lineReader = require('readline').createInterface({
            input: require('fs').createReadStream('datafile.txt')
        });

        let myjson;
        let cpt = 0;
        lineReader.on('line', function (line) {
            //console.log('Line from file:', line);
            myjson = JSON.parse(line);
            db.insert(myjson);
            if (cpt % 10 == 0)
                console.log(cpt);
            cpt += 1;
        });
        console.log("End")

    })();
});

//module.exports = class AI {

//    constructor() {
//        this.distance;
//        this.ennemyHP;
//        this.ennemyPA;
//        this.ennemyPM;
//        this.playerHP;
//        this.playerPA;
//        this.playerPM;
//    }
//}

var MongoClient = require('mongodb').MongoClient;
var MongoObjectID = require('mongodb').ObjectID;

class MyMongoDB2 {
    constructor() {
        this.client = null;
        this.mydb = null;
        this.trainningCollection = null;
    }

    async connection() {
        this.client = await MongoClient.connect("mongodb://localhost",
            { useNewUrlParser: true });

        this.mydb = this.client.db('DofusLike');

        this.trainningCollection = this.mydb.collection("trainning");
    }

    async insert(obj) {
        this.trainningCollection.insertOne(obj, function (err, res) {
            if (err) throw err;
            console.log("document inserted");
        });
    }
}