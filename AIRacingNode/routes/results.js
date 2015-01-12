var express = require('express');
var router = express.Router();

/* GET user profile. */
router.get('/', function(req, res) {
    var scripts = [];
    for (var scriptName in req.query.previous) {
        if (req.query.previous.hasOwnProperty(scriptName)) {
            scripts.push(req.query.previous[scriptName]);
        }
    }
    res.render('results', { scripts   : scripts,
                            positions : ["1st", "2nd", "3rd", "4th"]});
});

module.exports = router;
