<h1>Greetings Manager</h1>

<h2>Details</h2>
<p>For Alameda County ITD</p>

<h2>Environment</h2>
<ul>
  <li>.NET Core SDK 8.0.4</li>
  <li>VSCode 1.96.0</li>
  <li>Microsoft.EntityFrameworkCore.Sqlite 8.0.4</li>
  <li>Polly 8.5.0</li>
  <li>Swashbuckle.AspNetCore 6.6.2</li>
</ul>

<h3>Setup and Run API</h3>
<ul>
  <li>Clone the repository to your local machine</li>
  <li>In VSCode, open the terminal</li>
  <li>Run <code>dotnet restore</code></li>
  <li>Execute in the API folder: <code>dotnet run</code></li>
  <li>Execute in the client folder: <code>dotnet run</code></li>
  <li>Uses SQLite database as part of the package. All migrations have already been added, and it's set up and ready for use.</li>
</ul>

<h3>OpenAPI Documentation Based on OpenAPI Spec 3.0</h3>
<ul>
  <li>API documentation is available in OpenAPI spec at: <a href="http://localhost:5026/swagger" target="_blank">http://localhost:5026/swagger</a></li>
  <li>API testing can be performed using the interface provided at: <a href="http://localhost:5026/swagger" target="_blank">http://localhost:5026/swagger</a></li>
</ul>

<h3>Test for Rate Limiting</h3>
<p>The easiest way to test is by updating the rate limit parameters in <code>appsettings.Development.json</code>. Change the rate to extreme values like 3 requests per 10 seconds. Then, use the OpenAPI URL above to submit the requests. You should see 429 errors.</p>

<h2>Rate Limiting Middleware Explanation</h2>

<p>The <strong>RateLimitingMiddleware</strong> class is a custom middleware in an ASP.NET Core application designed to enforce rate limiting for incoming HTTP requests using Polly service.</p>
<ul>
  <li><strong>Constructor</strong>:
    <p>The constructor receives two parameters:</p>
    <ul>
      <li><strong>RequestDelegate next</strong>: This represents the next middleware in the pipeline.</li>
      <li><strong>AsyncRateLimitPolicy rateLimitPolicy</strong>: This is a Polly rate-limiting policy that defines the rate limit logic (e.g., maximum requests per time interval).</li>
    </ul>
  </li>
  <li><strong>InvokeAsync Method</strong>:
    <p>The <code>InvokeAsync</code> method is called for every HTTP request that passes through the middleware.</p>
    <p>Inside this method, the rate-limiting policy is executed using <code>await _rateLimitPolicy.ExecuteAsync</code>. If the rate limit is not exceeded, the request proceeds to the next middleware (<code>_next(context)</code>).</p>
    <p>If the rate limit is exceeded, a <strong>RateLimitRejectedException</strong> is thrown. The middleware then catches this exception and returns a <strong>429 Too Many Requests</strong> status code, with a message: "Rate limit exceeded. Try again later."</p>
  </li>
</ul>
<h3>Functionality:</h3>
<p>This middleware applies a rate-limiting policy to incoming requests, restricting the number of requests a client can make within a specified time window. If a client exceeds the allowed number of requests, they receive a <strong>429 Too Many Requests</strong> response, indicating they have hit the rate limit and should try again later.</p>
<p>In summary, the <strong>RateLimitingMiddleware</strong> helps protect the API from excessive traffic by enforcing a rate-limiting policy and ensuring a smooth user experience even when limits are reached.</p>

<h2>Memory Cache Explanation</h2>

<p>The <strong>MemoryCacheService</strong> class is an implementation of the <code>ICacheService</code> interface that provides in-memory caching functionality using ASP.NET Core's <code>IMemoryCache</code>.</p>

<h3>Key Methods:</h3>
<ul>
  <li><strong>Constructor (<code>MemoryCacheService</code>)</strong>:
    <p>Takes an <code>IMemoryCache</code> instance, which is injected into the class and used for caching operations.</p>
  </li>
  
  <li><strong>Get&lt;T&gt;</strong>:
    <p>Retrieves a cached value by its key. If the key exists in the cache, it returns the corresponding value of type <code>T</code>. Otherwise, it returns the default value for <code>T</code>.</p>
  </li>
  
  <li><strong>Set&lt;T&gt;</strong>:
    <p>Caches a value of type <code>T</code> with a specified key and expiration duration. It uses a sliding expiration (<code>SetSlidingExpiration</code>) to automatically remove the cached item after the specified time interval of inactivity.</p>
  </li>
  
  <li><strong>Remove</strong>:
    <p>Removes an item from the cache using its key.</p>
  </li>
</ul>

<h3>Purpose:</h3>
<p>The <strong>MemoryCacheService</strong> helps manage temporary data storage within the memory of the application, reducing the need for frequent database or API calls by caching data for quick retrieval, improving performance, and reducing latency. The controller code first checks if item exixts in cache and if not, it adds to it.</p>

