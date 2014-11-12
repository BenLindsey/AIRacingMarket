var express = require('express');
var router = express.Router();

router.post('/', function (req, res) {
    var collection = req.db.get('scores');
    console.log("help");
    collection.insert({
        "first"  : req.body.first,
        "second" : req.body.second
    }, function (err, doc) {
        if (err) {
            // If it failed, return error
            res.send("It broke dingus");
        }
        else {
            // If it worked, idk
            res.location("leaderboard");
            res.redirect("leaderboard");
        }
    });
});

module.exports = router; 
