// Mock server for testing endpoints without MongoDB
const express = require('express');
const app = express();

app.use(express.json());

// Mock data
let mockPlans = [
    {
        id: "plan-123",
        vvnId: "VVN-001",
        date: "2025-12-30",
        algorithm: "genetic",
        status: "ACTIVE",
        operations: [
            {
                operationId: "op-1",
                type: "LOADING",
                resourceId: "crane-001",
                startTime: "2025-12-30T08:00:00.000Z",
                endTime: "2025-12-30T10:00:00.000Z",
                status: "PLANNED"
            }
        ]
    }
];

// Simple counter to generate deterministic VVN IDs for the mock
let vvnCounter = 2;
function generateVvnId() {
    const id = `VVN-${String(vvnCounter).padStart(3, '0')}`;
    vvnCounter++;
    return id;
}

function nowIso() {
    return new Date().toISOString();
}

// Routes
app.post('/operation-plans/generate', (req, res) => {
    console.log('📝 POST /operation-plans/generate', req.body);
    // Validate required fields — treat empty object as missing
    if (!req.body || (typeof req.body === 'object' && Object.keys(req.body).length === 0) || !req.body.date || !req.body.algorithm) {
        return res.status(400).json({ error: 'Missing required field: date and algorithm are required' });
    }

    const newPlan = {
        id: `plan-${Date.now()}`,
        vvnId: "VVN-001", // keep for backward compatibility with existing tests that expect a VVN-
        date: req.body.date,
        algorithm: req.body.algorithm,
        status: "ACTIVE",
        operations: [
            {
                operationId: `op-${Date.now()}`,
                type: "LOADING",
                resourceId: "crane-001",
                startTime: `${req.body.date}T08:00:00.000Z`,
                endTime: `${req.body.date}T10:00:00.000Z`,
                status: "PLANNED"
            }
        ]
    };
    mockPlans.push(newPlan);
    res.status(201).json(newPlan);
});

// New: create operation plan (used by tests expecting createdAt and generated VVN)
app.post('/operation-plans', (req, res) => {
    console.log('🆕 POST /operation-plans', req.body);
    if (!req.body || (typeof req.body === 'object' && Object.keys(req.body).length === 0) || !req.body.date || !req.body.algorithm) {
        return res.status(400).json({ error: 'Missing required field: date and algorithm are required' });
    }

    const createdAt = nowIso();
    const newPlan = {
        id: `plan-${Date.now()}`,
        vvnId: generateVvnId(),
        date: req.body.date,
        algorithm: req.body.algorithm,
        status: "ACTIVE",
        createdAt,
        operations: [
            {
                operationId: `op-${Date.now()}`,
                type: "LOADING",
                resourceId: "crane-001",
                startTime: `${req.body.date}T08:00:00.000Z`,
                endTime: `${req.body.date}T10:00:00.000Z`,
                status: "PLANNED"
            }
        ]
    };
    mockPlans.push(newPlan);
    res.status(201).json(newPlan);
});

app.get('/operation-plans/all', (req, res) => {
    console.log('📋 GET /operation-plans/all');
    res.json(mockPlans);
});

// New: Get plans by date range (placed before param routes)
app.get('/operation-plans/date-range/:start/:end', (req, res) => {
    console.log('📆 GET /operation-plans/date-range', req.params.start, req.params.end);
    const { start, end } = req.params;
    if (!start || !end || start > end) {
        return res.status(400).json({ error: 'Invalid date range' });
    }
    const results = mockPlans.filter(p => p.date >= start && p.date <= end);
    res.json(results);
});

// New: Get plans by vessel VVN ID (placed before param routes)
app.get('/operation-plans/vessel/:vvnId', (req, res) => {
    console.log('🚢 GET /operation-plans/vessel', req.params.vvnId);
    const vvnId = req.params.vvnId;
    const results = mockPlans.filter(p => p.vvnId === vvnId);
    res.json(results);
});

app.get('/operation-plans/:id', (req, res) => {
    console.log('🔍 GET /operation-plans/:id', req.params.id);
    const plan = mockPlans.find(p => p.id === req.params.id);
    if (!plan) {
        return res.status(404).json({ error: 'Plan not found' });
    }
    res.json(plan);
});

app.put('/operation-plans/:id', (req, res) => {
    console.log('✏️ PUT /operation-plans/:id', req.params.id, req.body);
    const planIndex = mockPlans.findIndex(p => p.id === req.params.id);
    if (planIndex === -1) {
        return res.status(404).json({ error: 'Plan not found' });
    }

    // Apply updates
    const plan = mockPlans[planIndex];
    if (req.body.operations) {
        req.body.operations.forEach(update => {
            const opIndex = plan.operations.findIndex(op => op.operationId === update.operationId);
            if (opIndex !== -1) {
                plan.operations[opIndex] = { ...plan.operations[opIndex], ...update };
            }
        });
    }

    // Persist changes in mockPlans (plan is a reference so this is already persisted)

    const response = {
        ...plan,
        lastModified: nowIso(),
        lastModifiedBy: req.body.author,
        modificationReason: req.body.reason,
        warnings: ["⚠️ This is a mock response - MongoDB not connected"]
    };

    res.json(response);
});

// New: Delete plan by id
app.delete('/operation-plans/:id', (req, res) => {
    console.log('🗑️ DELETE /operation-plans/:id', req.params.id);
    const planIndex = mockPlans.findIndex(p => p.id === req.params.id);
    if (planIndex === -1) {
        return res.status(404).json({ error: 'Plan not found' });
    }
    mockPlans.splice(planIndex, 1);
    res.status(204).send();
});

app.listen(3000, () => {
    console.log('🚀 Mock server running on http://localhost:3000');
    console.log('📝 Available endpoints:');
    console.log('   POST /operation-plans/generate');
    console.log('   POST /operation-plans');
    console.log('   GET  /operation-plans/all');
    console.log('   GET  /operation-plans/:id');
    console.log('   GET  /operation-plans/date-range/:start/:end');
    console.log('   GET  /operation-plans/vessel/:vvnId');
    console.log('   PUT  /operation-plans/:id');
    console.log('   DELETE /operation-plans/:id');
    console.log('');
    console.log('💡 Test with:');
    console.log('   curl -X POST http://localhost:3000/operation-plans/generate \\');
    console.log('     -H "Content-Type: application/json" \\');
    console.log('     -d \'{"date": "2025-12-30", "algorithm": "genetic"}\'');
});
