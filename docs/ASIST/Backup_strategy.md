# ASIST – Backup Strategy, RPO & WRT

This User Story requires a complete backup strategy that is proposed and justified, is practically implementable, and minimizes **RPO** (how much data can be lost) and **WRT** (how long it takes to recover the system), while enabling rapid system restoration after a failure.

The presented strategy fulfills this objective by clearly defining:
- what is backed up,
- how frequently backups are performed,
- how long backups are retained,
- how the system is restored,
- and what the RPO and WRT values are, with technical justification.

## What is Included in the Backup

The strategy begins by identifying what is essential for the system to function.

### Database (PostgreSQL)
- Contains all operational data.
- Is the most critical element.
- Is always included in the backup.

### Application Configuration
- `docker-compose.yml`
- `.env` files

Without these files, it is not possible to rebuild the environment on a new machine.

### Optional Data
- Public folders or shared files.
- Not essential for minimum continuity, but can be included if necessary.

> This shows that the strategy focuses on what's essential for rapid recovery, reducing WRT.

## Backup Process

A restore example script exists `/usr/local/bin/backup.sh`, but restores may also be executed manually.

Backups are performed by automated scripts, ensuring repeatability and reducing human error.

### Database Backup

The database backup is performed with the following command:

```bash
pg_dump -h DB_HOST -U DB_USER DB_NAME | gzip > /srv/backups/db-DBNAME-TIMESTAMP.sql.gz
```

**Command explanation:**
- `pg_dump` - Official PostgreSQL tool to export the entire database structure and data.
- `-h DB_HOST` - Indicates the server where the database is running.
- `-U DB_USER` - User with permissions to perform the dump.
- `DB_NAME` - Name of the database to be copied.
- `| gzip` - The output is compressed in real-time, reducing disk space usage.
- `/srv/backups/db-DBNAME-TIMESTAMP.sql.gz` - Backup destination file with timestamp, allowing exact identification of the backup moment.

This method ensures a consistent backup that is easy to restore and suitable for meeting the defined RPO.

### Application Configuration Backup

The configuration backup is performed with the following command:

```bash
tar czf /srv/backups/configs-TIMESTAMP.tar.gz \
/srv/project/docker-compose.yml \
/srv/project/.env
```

**Command explanation:**
- `tar` - Tool used to group multiple files into a single archive.
- `c` - Creates a new archive.
- `z` - Applies gzip compression.
- `f` - Defines the output filename.
- `/srv/backups/configs-TIMESTAMP.tar.gz` - Final archive with timestamp, allowing backup versioning.

The included files enable complete reconstruction of the application's Docker environment.

## Frequency and Retention

### Frequency
- Daily backup of database and configurations.
- Additional backups before important changes.

### Retention
- 14 days of backups are maintained.
- Old backups are removed.

This policy balances:
- disk space,
- security,
- recovery capability.

## Restoration Procedures

### Steps
1. Select the correct backup.
2. Restore the database.
3. Restore the configurations.
4. Redeploy the system via Docker or CI/CD.

### Database Restoration

A restore example script exists `/usr/local/bin/restore.sh`, but restores may also be executed manually.


```bash
gunzip -c db-DBNAME-TIMESTAMP.sql.gz | psql -h DB_HOST -U DB_USER DB_NAME
```

**Command explanation:**
- `gunzip -c` - Decompresses the `.gz` file and sends the content directly to standard output, without creating a temporary file on disk.
- `db-DBNAME-TIMESTAMP.sql.gz` - Database backup file, identified by name and timestamp.
- `| psql` - The decompressed content is passed directly to the PostgreSQL client.
- `-h DB_HOST` - Indicates the server where the database will be restored.
- `-U DB_USER` - User with permissions to restore data.
- `DB_NAME` - Destination database.

This method allows for quick, efficient, and secure restoration, reducing the time needed to recover data.

### Configuration Files Restoration

```bash
tar xzf configs-TIMESTAMP.tar.gz -C /
```

**Command explanation:**
- `tar` - Tool used to extract archives.
- `x` - Indicates file extraction.
- `z` - Specifies that the archive is compressed with gzip.
- `f` - Defines the input file.
- `configs-TIMESTAMP.tar.gz` - Archive containing configuration files.
- `-C /` - Extracts files to their original paths in the system.

This step ensures that the application environment (Docker, environment variables, services) is exactly as it was at the backup moment.

### Validation

The restoration is considered valid if:
- the database loads correctly,
- the application starts,
- essential operations function.

## RPO – Recovery Point Objective

**RPO = 24 hours**

This means:
- in the worst-case scenario, a maximum of 1 day of data is lost,
- value consistent with daily backups,
- easily improvable with higher backup frequency.

## WRT – Work Recovery Time

**WRT = ≤ 1 hour**

This time includes:
- restoring database,
- restoring configurations,
- starting containers,
- validating functionality.

Thanks to:
- simple backups,
- automated scripts,
- Docker and CI/CD.


## Validation of the Strategy

This strategy is validated when:

- A full backup–restore cycle is tested successfully.
- Documentation is updated based on the results.
- Backup and restore operations remain consistent as the system evolves.



## Future Improvements

Potential improvements include:

- Automated off-machine backups (e.g., rsync to another VM).
- Backup integrity verification.
- Encryption of backup archives.
- More frequent incremental backups.



## Final Conclusion

The defined backup strategy offers a clear, automatable, and dependable approach for preserving and restoring system data.  
The defined RPO (24h) and WRT (≤1h) align with the MBCO and provide a realistic balance between simplicity and reliability in the context of the project.