var express = require('express');
var router = express.Router();

/* GET user profile. */
router.get('/', function(req, res) {
    res.render('results');
});

module.exports = router;