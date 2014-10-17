var mongoose = require('mongoose');

// define the schema for our user model
var timeSchema = mongoose.Schema({
    time  : {
        username     : String,
        scriptName   : String,
        scriptTime   : Number
    }
});

// create the model for users and expose it to our app
module.exports = mongoose.model('Time', timeSchema);