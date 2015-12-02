self.addEventListener('message', function(e) {
  console.log('doWork');
  self.postMessage(e.data);
}, false);