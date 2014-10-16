var express = require('express');
var router = express.Router();

/* GET New User page. */
router.get('/', function(req, res) {
    res.render('login', { message: req.flash('loginMessage') });
});

module.exports = router;
