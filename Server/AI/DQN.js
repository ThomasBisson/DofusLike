//import * as tf from '@tensorflow/tfjs-node'
var tf = require('@tensorflow/tfjs-node')

module.exports = class DQN {
    constructor() {
        this.model;
        this.BuildModel();
    }

    getDataset() {
        let row_per_class = 100;
        let faetures = [];
        for(let i =0; i<row_per_class; i++) {
            faetures.push([Math.random() - 2, Math.random() - 2]);
        }

        for(let i =0; i<row_per_class; i++) {
            faetures.push([Math.random() + 2, Math.random() + 2]);
        }

        for(let i=0; i<row_per_class; i++) {
            faetures.push([Math.random() - 2, Math.random() + 2]);
        }

        for(let i=0; i<row_per_class; i++) {
            faetures.push([Math.random() + 2, Math.random() - 2]);
        }

        let targets = [];
        for(let i =0; i< 200; i++) {
            targets.push(0);
        }
        for(let i=0; i< 200; i++) {
            targets.push(1);
        }

        return [faetures, targets];
    }

    BuildModel() {

        this.model = tf.sequential({
            layers: [
                tf.layers.dense({inputShape: 2, activation: 'sigmoid', units: 3 }),
                tf.layers.dense({inputShape: 3, activation: 'sigmoid', units: 1 }),
            ]
        });

        this.model.compile({
            loss: 'meanSquaredError',
            optimizer: tf.train.adam(0.01),
            metrics: ['accuracy']
        });

        //this.model.summary();
    }

    // predicting model
    async TrainData(trainingData, outputData) {

        let tensorTraining = tf.tensor(trainingData);
        let tensorOutput = tf.tensor(outputData);

        console.log('......Loss and Accuracy History.......');
        for(let i=0;i<10;i++){
            await this.model.fit(tensorTraining, tensorOutput, {
                epochs: 40
            });
        };
        //console.log(`Iteration ${i}: [LOSS] ${mean(res.history.loss)}`);         
        //console.log(`Iteration ${i}: [ACC] ${mean(res.history.acc)}`);         
        //console.log();
    }

    Predict(data) {
        let tensorData = tf.tensor(data);
        return this.model.predict(tensorData);
    } 

    mean(array){
        if (array.length == 0)
            return null;
        var sum = array.reduce(function(a, b) { return a + b; });
        var avg = sum / array.length;
        return avg;
    }

}