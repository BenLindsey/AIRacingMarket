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
                            positions : ["First", "Second", "Third", "Fourth"]});
});

module.exports = router;
