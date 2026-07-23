// test-behavior.js
// Testes completos que verificam COMPORTAMENTO e LÓGICA DE NEGÓCIO

console.log('🚨 TEST FILE STARTED');


const BASE_URL = 'http://localhost:3000';

// Cores para output
const colors = {
  reset: '\x1b[0m',
  green: '\x1b[32m',
  red: '\x1b[31m',
  yellow: '\x1b[33m',
  blue: '\x1b[34m'
};

// Helper para assertions
function assert(condition, testName, actual, expected) {
  if (condition) {
    console.log(`${colors.green}✅ PASS${colors.reset} - ${testName}`);
    return true;
  } else {
    console.log(`${colors.red}❌ FAIL${colors.reset} - ${testName}`);
    console.log(`   Expected: ${JSON.stringify(expected)}`);
    console.log(`   Got: ${JSON.stringify(actual)}`);
    return false;
  }
}

async function runBehaviorTests() {
  console.log('╔════════════════════════════════════════════════╗');
  console.log('║   🧪 BEHAVIOR TESTS - Operation Plans         ║');
  console.log('╚════════════════════════════════════════════════╝\n');

  let passed = 0;
  let failed = 0;

  try {
    // ============================================
// TEST SUITE 1: CREATE OPERATION PLAN
// ============================================
console.log(`${colors.blue}📦 Test Suite 1: Create Operation Plan${colors.reset}\n`);

// Test 1.1: Criar plan com dados válidos
console.log('Test 1.1: Create with valid data');
const createRes = await fetch(`${BASE_URL}/operation-plans/generate`, {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ date: "2025-12-31", algorithm: "genetic" })
});
const plan = await createRes.json();

assert(createRes.status === 201, 'HTTP 201 Created', createRes.status, 201) ? passed++ : failed++;
assert(plan.id !== undefined, 'Plan has ID', plan.id !== undefined, true) ? passed++ : failed++;
assert(plan.date === "2025-12-31", 'Correct date', plan.date, "2025-12-31") ? passed++ : failed++;
assert(plan.algorithm === "genetic", 'Correct algorithm', plan.algorithm, "genetic") ? passed++ : failed++;
assert(plan.status === "ACTIVE", 'Initial status is ACTIVE', plan.status, "ACTIVE") ? passed++ : failed++;
assert(Array.isArray(plan.operations), 'Operations is array', Array.isArray(plan.operations), true) ? passed++ : failed++;
assert(plan.operations.length > 0, 'Has at least one operation', plan.operations.length > 0, true) ? passed++ : failed++;

const planId = plan.id;

// Test 1.2: Criar plan sem campos obrigatórios
console.log('\nTest 1.2: Create without required fields');
const invalidRes = await fetch(`${BASE_URL}/operation-plans/generate`, {  // <-- endpoint corrigido
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({}) // Sem date e algorithm
});

assert(invalidRes.status === 400, 'HTTP 400 Bad Request', invalidRes.status, 400) ? passed++ : failed++;

// Test 1.3: Verificar estrutura das operações
console.log('\nTest 1.3: Verify operation structure');
const op = plan.operations[0];
assert(op.operationId !== undefined, 'Operation has ID', op.operationId !== undefined, true) ? passed++ : failed++;
assert(op.type !== undefined, 'Operation has type', op.type !== undefined, true) ? passed++ : failed++;
assert(op.resourceId !== undefined, 'Operation has resourceId', op.resourceId !== undefined, true) ? passed++ : failed++;
assert(op.startTime !== undefined, 'Operation has startTime', op.startTime !== undefined, true) ? passed++ : failed++;
assert(op.endTime !== undefined, 'Operation has endTime', op.endTime !== undefined, true) ? passed++ : failed++;

    // ============================================
    // TEST SUITE 2: READ OPERATION PLANS
    // ============================================
    console.log(`\n${colors.blue}📖 Test Suite 2: Read Operation Plans${colors.reset}\n`);

    // Test 2.1: Listar todos os plans
    console.log('Test 2.1: Get all plans');
    const allRes = await fetch(`${BASE_URL}/operation-plans/all`);
    const allPlans = await allRes.json();
    
    assert(allRes.status === 200, 'HTTP 200 OK', allRes.status, 200) ? passed++ : failed++;
    assert(Array.isArray(allPlans), 'Response is array', Array.isArray(allPlans), true) ? passed++ : failed++;
    assert(allPlans.length > 0, 'Has at least one plan', allPlans.length > 0, true) ? passed++ : failed++;

    // Test 2.2: Buscar plan por ID existente
    console.log('\nTest 2.2: Get plan by existing ID');
    const getRes = await fetch(`${BASE_URL}/operation-plans/${planId}`);
    const gotPlan = await getRes.json();
    
    assert(getRes.status === 200, 'HTTP 200 OK', getRes.status, 200) ? passed++ : failed++;
    assert(gotPlan.id === planId, 'Correct plan ID', gotPlan.id, planId) ? passed++ : failed++;

    // Test 2.3: Buscar plan por ID inexistente
    console.log('\nTest 2.3: Get plan by non-existent ID');
    const notFoundRes = await fetch(`${BASE_URL}/operation-plans/plan-999999`);
    
    assert(notFoundRes.status === 404, 'HTTP 404 Not Found', notFoundRes.status, 404) ? passed++ : failed++;

    // Test 2.4: Buscar por intervalo de datas
    console.log('\nTest 2.4: Get plans by date range');
    const dateRangeRes = await fetch(`${BASE_URL}/operation-plans/date-range/2025-12-01/2025-12-31`);
    const dateRangePlans = await dateRangeRes.json();
    
    assert(dateRangeRes.status === 200, 'HTTP 200 OK', dateRangeRes.status, 200) ? passed++ : failed++;
    assert(Array.isArray(dateRangePlans), 'Response is array', Array.isArray(dateRangePlans), true) ? passed++ : failed++;
    
    // Verificar que todos os plans estão no intervalo
    const allInRange = dateRangePlans.every(p => 
      p.date >= "2025-12-01" && p.date <= "2025-12-31"
    );
    assert(allInRange, 'All plans in date range', allInRange, true) ? passed++ : failed++;

    // Test 2.5: Buscar por vessel ID
    console.log('\nTest 2.5: Get plans by vessel ID');
    const vesselRes = await fetch(`${BASE_URL}/operation-plans/vessel/${plan.vvnId}`);
    const vesselPlans = await vesselRes.json();
    
    assert(vesselRes.status === 200, 'HTTP 200 OK', vesselRes.status, 200) ? passed++ : failed++;
    assert(Array.isArray(vesselPlans), 'Response is array', Array.isArray(vesselPlans), true) ? passed++ : failed++;

    // ============================================
    // TEST SUITE 3: UPDATE OPERATION PLAN
    // ============================================
    console.log(`\n${colors.blue}✏️  Test Suite 3: Update Operation Plan${colors.reset}\n`);

    // Test 3.1: Atualizar operação existente
    console.log('Test 3.1: Update existing operation');
    const updateRes = await fetch(`${BASE_URL}/operation-plans/${planId}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        operations: [{
          operationId: plan.operations[0].operationId,
          startTime: "2025-12-31T14:00:00.000Z",
          endTime: "2025-12-31T16:00:00.000Z",
          resourceId: "crane-updated",
          status: "IN_PROGRESS"
        }],
        author: "test_user",
        reason: "Testing update"
      })
    });
    const updatedPlan = await updateRes.json();
    
    assert(updateRes.status === 200, 'HTTP 200 OK', updateRes.status, 200) ? passed++ : failed++;
    assert(updatedPlan.lastModified !== undefined, 'Has lastModified', updatedPlan.lastModified !== undefined, true) ? passed++ : failed++;
    assert(updatedPlan.lastModifiedBy === "test_user", 'Correct author', updatedPlan.lastModifiedBy, "test_user") ? passed++ : failed++;
    assert(updatedPlan.modificationReason === "Testing update", 'Correct reason', updatedPlan.modificationReason, "Testing update") ? passed++ : failed++;
    
    // Verificar se a operação foi atualizada
    const updatedOp = updatedPlan.operations.find(op => op.operationId === plan.operations[0].operationId);
    assert(updatedOp.resourceId === "crane-updated", 'Resource updated', updatedOp.resourceId, "crane-updated") ? passed++ : failed++;
    assert(updatedOp.status === "IN_PROGRESS", 'Status updated', updatedOp.status, "IN_PROGRESS") ? passed++ : failed++;

    // Test 3.2: Tentar atualizar plan inexistente
    console.log('\nTest 3.2: Update non-existent plan');
    const updateNotFoundRes = await fetch(`${BASE_URL}/operation-plans/plan-999999`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ operations: [] })
    });
    
    assert(updateNotFoundRes.status === 404, 'HTTP 404 Not Found', updateNotFoundRes.status, 404) ? passed++ : failed++;

    // ============================================
    // TEST SUITE 4: DELETE OPERATION PLAN
    // ============================================
    console.log(`\n${colors.blue}🗑️  Test Suite 4: Delete Operation Plan${colors.reset}\n`);

    // Test 4.1: Deletar plan existente
    console.log('Test 4.1: Delete existing plan');
    const deleteRes = await fetch(`${BASE_URL}/operation-plans/${planId}`, {
      method: 'DELETE'
    });
    
    assert(deleteRes.status === 204, 'HTTP 204 No Content', deleteRes.status, 204) ? passed++ : failed++;

    // Test 4.2: Verificar que plan foi deletado
    console.log('\nTest 4.2: Verify plan was deleted');
    const verifyDeleteRes = await fetch(`${BASE_URL}/operation-plans/${planId}`);
    
    assert(verifyDeleteRes.status === 404, 'Plan not found after delete', verifyDeleteRes.status, 404) ? passed++ : failed++;

    // Test 4.3: Tentar deletar plan inexistente
    console.log('\nTest 4.3: Delete non-existent plan');
    const deleteNotFoundRes = await fetch(`${BASE_URL}/operation-plans/plan-999999`, {
      method: 'DELETE'
    });
    
    assert(deleteNotFoundRes.status === 404, 'HTTP 404 Not Found', deleteNotFoundRes.status, 404) ? passed++ : failed++;

    // ============================================
    // TEST SUITE 5: BUSINESS LOGIC
    // ============================================
    console.log(`\n${colors.blue}🧠 Test Suite 5: Business Logic${colors.reset}\n`);

    // Test 5.1: Data de criação deve estar presente
    console.log('Test 5.1: Creation date validation');
    const newPlanRes = await fetch(`${BASE_URL}/operation-plans`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ date: "2026-01-01", algorithm: "heuristic" })
    });
    const newPlan = await newPlanRes.json();
    
    assert(newPlan.createdAt !== undefined, 'Has createdAt timestamp', newPlan.createdAt !== undefined, true) ? passed++ : failed++;

    // Test 5.2: VVN ID deve ser gerado
    console.log('\nTest 5.2: VVN ID generation');
    assert(newPlan.vvnId !== undefined, 'Has VVN ID', newPlan.vvnId !== undefined, true) ? passed++ : failed++;
    assert(newPlan.vvnId.startsWith('VVN-'), 'VVN ID format', newPlan.vvnId.startsWith('VVN-'), true) ? passed++ : failed++;

    // Test 5.3: Operações devem ter timestamps válidos
    console.log('\nTest 5.3: Operation timestamps validation');
    const operation = newPlan.operations[0];
    const startTime = new Date(operation.startTime);
    const endTime = new Date(operation.endTime);
    
    assert(endTime > startTime, 'End time after start time', endTime > startTime, true) ? passed++ : failed++;

    // ============================================
    // FINAL REPORT
    // ============================================
    console.log('\n╔════════════════════════════════════════════════╗');
    console.log('║              📊 TEST RESULTS                   ║');
    console.log('╚════════════════════════════════════════════════╝');
    console.log(`${colors.green}✅ Passed: ${passed}${colors.reset}`);
    console.log(`${colors.red}❌ Failed: ${failed}${colors.reset}`);
    console.log(`Total: ${passed + failed} tests`);
    console.log(`Success Rate: ${((passed / (passed + failed)) * 100).toFixed(2)}%\n`);

    if (failed === 0) {
      console.log(`${colors.green}🎉 All tests passed!${colors.reset}\n`);
    } else {
      console.log(`${colors.yellow}⚠️  Some tests failed. Review the output above.${colors.reset}\n`);
    }

  } catch (error) {
    console.error(`\n${colors.red}❌ Test suite crashed:${colors.reset}`, error.message);
    console.log('\n💡 Troubleshooting:');
    console.log('   1. Make sure mock server is running: node mock-server.js');
    console.log('   2. Check if port 3000 is available');
    console.log('   3. Verify no firewall blocking localhost:3000');
  }
}

// Run behavior tests
runBehaviorTests();