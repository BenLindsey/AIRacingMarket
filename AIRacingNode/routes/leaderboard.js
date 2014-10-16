var express = require('express');
var router = express.Router();

/* GET times */
router.get('/', function(req, res) {
    var db = req.db;
    var collection = db.get('timecollection');
    collection.find({}, {}, function(e, docs) {
        res.render('leaderboard', {
            "leaderboard" : docs
        });
    });
});

module.exports = router;
