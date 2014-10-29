var express = require('express');
var router = express.Router();

/* GET Times */
router.get('/', function(req, res) {
    var collection = req.db.get('scriptcollection');

    collection.find({}, {sort : { scriptName : 1 }}, function(e, docs) {
        res.render('admin', {
            "scripts" : docs
        });
    });
});

module.exports = router;
