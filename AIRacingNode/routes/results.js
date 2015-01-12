var express = require('express');
var router = express.Router();

/* GET user profile. */
router.get('/', function(req, res) {
    res.render('results', {scripts : req.query.scripts});
});

module.exports = router;
