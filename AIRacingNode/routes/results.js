var express = require('express');
var router = express.Router();

/* GET user profile. */
router.get('/', function(req, res) {
    var scripts = [];
    for (var scriptName in req.query.scripts) {
        if (req.query.scripts.hasOwnProperty(scriptName)) {
            scripts.push(req.query.scripts[scriptName]);
        }
    }
    res.render('results', { scripts   : scripts,
                            positions : ["1st", "2nd", "3rd", "4th"]});
});

module.exports = router;
