-- Create databases for microservices
CREATE DATABASE filestoring_db;
CREATE DATABASE fileanalysis_db;

-- Grant permissions to postgres user (which already exists)
GRANT ALL PRIVILEGES ON DATABASE filestoring_db TO postgres;
GRANT ALL PRIVILEGES ON DATABASE fileanalysis_db TO postgres;

-- Connect to filestoring_db and grant schema permissions
\c filestoring_db;
GRANT ALL ON SCHEMA public TO postgres;

-- Connect to fileanalysis_db and grant schema permissions
\c fileanalysis_db;
GRANT ALL ON SCHEMA public TO postgres;