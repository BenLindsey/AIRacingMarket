var express = require('express');
var router = express.Router();

/* GET user profile. */
router.get('/', function(req, res) {
    var scripts = [];
    for (var scriptName in req.query.scripts) {
        scripts.push(scriptName);
    }
    res.render('results', {scripts : scripts});
});

module.exports = router;
