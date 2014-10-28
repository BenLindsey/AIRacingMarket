var Browser = require("zombie");
var assert = require("assert");

describe('login page', function() {

  before(function() {
    this.browser = new Browser({site : 'http://localhost:3013' });
  });

  before(function(done) {
    this.browser.visit('/login', done);
  });

  it('should redirect to home on login', function() {
  this.browser.
    fill("email", "a@a.com").
    fill("password", "a").
    pressButton("Login", function() {
      assert.ok(browser.success);
      assert.equal(browser.text("title"), "AI Racing Market");
    })
  });

});
