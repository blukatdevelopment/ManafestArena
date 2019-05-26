# Refactors

## Data storage
Currently there are csv files as far as the eye can see. This is gross, and should be replaced. When files are supposed to be human-editable and small, JSON would be ideal. When files are supposed to be giant blobs of data (such as terrain), binary formatting sounds like the ticket.