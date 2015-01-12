var express = require('express');
var router = express.Router();

/* GET user profile. */
router.get('/', function(req, res) {
    var scripts = [];
    console.log("queryScripts: " + req.query.scripts);
    for (var scriptName in req.query.scripts) {
        scripts.push(req.query.scripts.scriptName);
    }
    console.log("scripts: " + scripts);
    res.render('results', {scripts : scripts});
});

module.exports = router;
