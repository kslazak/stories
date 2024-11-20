Feature: Best stories

Scenario: Best stories requested with count set to 1
	When GetBestStories is called with count parameter equal to '1'
	Then the following results are being returned
		| Title     | Uri     | PostedBy | Time                      | Score | CommentCount |
		| Title 200 | Uri 200 | User 200 | 1970-01-01T00:03:20+00:00 | 200   | 200          |

Scenario: Best stories requested with count set to 5
	When GetBestStories is called with count parameter equal to '5'
	Then the following results are being returned
		| Title     | Uri     | PostedBy | Time                      | Score | CommentCount |
		| Title 200 | Uri 200 | User 200 | 1970-01-01T00:03:20+00:00 | 200   | 200          |
		| Title 199 | Uri 199 | User 199 | 1970-01-01T00:03:19+00:00 | 199   | 199          |
		| Title 198 | Uri 198 | User 198 | 1970-01-01T00:03:18+00:00 | 198   | 198          |
		| Title 197 | Uri 197 | User 197 | 1970-01-01T00:03:17+00:00 | 197   | 197          |
		| Title 196 | Uri 196 | User 196 | 1970-01-01T00:03:16+00:00 | 196   | 196          |

Scenario: Best stories requested with count set to 200
	When GetBestStories is called with count parameter equal to '200'
	Then 200 results are being returned

Scenario: Best stories requested with count set to 0
	When GetBestStories is called with invalid count parameter equal to '0'
	Then the 'Request failed with status code BadRequest' error is being returned

Scenario: Best stories requested with count set to 201
	When GetBestStories is called with invalid count parameter equal to '201'
	Then the 'Request failed with status code BadRequest' error is being returned

Scenario: Best stories requested with count set to -1
	When GetBestStories is called with invalid count parameter equal to '-1'
	Then the 'Request failed with status code BadRequest' error is being returned

Scenario: Best stories requested with count set to A
	When GetBestStories is called with invalid count parameter equal to 'A'
	Then the 'Request failed with status code BadRequest' error is being returned

Scenario: Best stories requested with count set to empty value
	When GetBestStories is called with invalid count parameter equal to ''
	Then the 'Request failed with status code BadRequest' error is being returned

Scenario: Best stories requested without count
	When GetBestStories is called without count parameter
	Then the 'Request failed with status code BadRequest' error is being returned