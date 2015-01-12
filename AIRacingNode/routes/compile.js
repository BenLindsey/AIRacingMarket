var express = require('express');
var router = express.Router();
var coffeeScript = require('coffee-script');

router.post('/', function(req, res) {
    res.send(coffeeScript.compile(req.body.script, {bare: true}), 200);
});

module.exports = router;
