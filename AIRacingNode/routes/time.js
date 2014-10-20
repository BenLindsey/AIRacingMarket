var express = require('express');
var router = express.Router();

router.post('/', function (req, res) {
    var collection = req.db.get('timecollection');

    collection.insert({
        "scriptName" : req.body.scriptname,
        "levelName"  : req.body.levelname,
        "time"       : parseFloat(req.body.time)
    }, function (err, doc) {
        if (err) {
            // If it failed, return error
            res.send("There was a problem adding the information to the database.");
        }
        else {
            // If it worked, set the header so the address bar doesn't still say /adduser
            res.location("leaderboard");
            // And forward to success page
            res.redirect("leaderboard");
        }
    });
});

module.exports = router;
