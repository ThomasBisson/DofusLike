module.exports = {

    sleepTurn: function (ms) {
        return new Promise(resolve => {
            setTimeout(resolve, ms);
        });
    }

}