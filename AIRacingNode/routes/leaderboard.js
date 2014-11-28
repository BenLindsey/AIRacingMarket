var express = require('express');
var router = express.Router();

/* GET Times */
router.get('/', function(req, res) {
    var collection = req.db.get('scores');

    collection.find({}, {sort : { rating : -1 }}, function(e, docs) {
        res.render('leaderboard', {
            "leaderboard" : docs
        });
    });
});

module.exports = router;
