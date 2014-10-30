var express = require('express');
var router = express.Router();

/* GET user profile. */
router.get('/', isLoggedIn, function(req, res) {
    var scriptNames = req.query.scripts;

    var scripts = [];

    for(var key in scriptNames) {
        scripts.push({name: scriptNames[key], content:""});
    }

    res.render('webbuild', { scripts : scripts, levelname : req.query.levelname } );
});

function isLoggedIn(req, res, next) {
    req.session.redirect = '/webbuild';

    if (req.isAuthenticated())
        return next();

    res.redirect('/login');
}

module.exports = router;
