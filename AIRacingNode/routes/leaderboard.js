var express = require('express');
var router = express.Router();

/* GET Times */
router.get('/', function(req, res) {
    var db = req.db;
    var collection = db.get('timecollection');
    collection.find({}, {sort : { scriptTime : 1 }}, function(e, docs) {
        res.render('leaderboard', {
            "leaderboard" : docs
        });
    });
});

module.exports = router;
