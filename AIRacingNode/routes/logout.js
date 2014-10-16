var express = require('express');
var router = express.Router();

/* GET New User page. */
router.get('/', function(req, res) {
    req.logout();
    res.redirect('/index');
});

module.exports = router;
