#!/bin/bash

# ============================================
# IARTI Algorithms - Enhanced Test Suite v2.0
# ============================================

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
MAGENTA='\033[0;35m'
NC='\033[0m'

# Configuration
API_URL="http://localhost:5001"
FIXTURES_DIR="tests/fixtures/iarti"
RESULTS_DIR="tests/results/iarti"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
REPORT_FILE="$RESULTS_DIR/test_report_$TIMESTAMP.txt"
SUMMARY_FILE="$RESULTS_DIR/test_summary_$TIMESTAMP.csv"

mkdir -p "$FIXTURES_DIR" "$RESULTS_DIR"

# ============================================
# HEADER
# ============================================
echo "==========================================" | tee "$REPORT_FILE"
echo "  IARTI Algorithms - Enhanced Test Suite" | tee -a "$REPORT_FILE"
echo "  v2.0 - Algorithm Validation Enabled" | tee -a "$REPORT_FILE"
echo "  $(date)" | tee -a "$REPORT_FILE"
echo "==========================================" | tee -a "$REPORT_FILE"
echo "" | tee -a "$REPORT_FILE"

# Check server
if ! curl -s "$API_URL/health" > /dev/null 2>&1; then
    echo -e "${RED}ERROR: Server not running on port 5001${NC}" | tee -a "$REPORT_FILE"
    echo "Please start the server first:" | tee -a "$REPORT_FILE"
    echo "  cd prolog && swipl run.pl" | tee -a "$REPORT_FILE"
    exit 1
fi

# Validate server version
server_info=$(curl -s "$API_URL/health")
server_module=$(echo "$server_info" | jq -r '.module')
if [ "$server_module" != "scheduler-backend-iarti" ]; then
    echo -e "${YELLOW}WARNING: Unexpected server module: $server_module${NC}" | tee -a "$REPORT_FILE"
fi

echo -e "${GREEN}✓ Server is running: $server_module${NC}" | tee -a "$REPORT_FILE"
echo "" | tee -a "$REPORT_FILE"

# Counters
total_tests=0
passed_tests=0
failed_tests=0
warning_tests=0

# CSV Header
echo "TestName,Endpoint,ExpectedAlgorithm,ActualAlgorithm,Delay,ComputationTime,Success,AlgoMatch" > "$SUMMARY_FILE"

# ============================================
# FUNCTION: Generate Test Fixtures
# ============================================
generate_fixtures() {
    echo -e "${BLUE}Generating test fixtures...${NC}" | tee -a "$REPORT_FILE"
    
    # 1. Small (3 vessels) - brute-force expected
    cat > "$FIXTURES_DIR/test-small-3v.json" << 'EOF'
{"requestId":"small-3v","vessels":[{"imo":"IMO001","eta":360,"etd":720,"cargo":100},{"imo":"IMO002","eta":480,"etd":840,"cargo":150},{"imo":"IMO003","eta":600,"etd":960,"cargo":50}],"resources":[{"code":"C1","capacity":30,"status":"available"}],"staff":[{"id":"S1","status":"available"}],"storage_areas":[{"code":"Y1","type":"yard","capacity":1000}]}
EOF

    # 2. Medium (8 vessels) - brute-force expected (limit)
    cat > "$FIXTURES_DIR/test-medium-8v.json" << 'EOF'
{"requestId":"medium-8v","vessels":[{"imo":"IMO001","eta":0,"etd":300,"cargo":100},{"imo":"IMO002","eta":60,"etd":360,"cargo":120},{"imo":"IMO003","eta":120,"etd":420,"cargo":90},{"imo":"IMO004","eta":180,"etd":480,"cargo":110},{"imo":"IMO005","eta":240,"etd":540,"cargo":130},{"imo":"IMO006","eta":300,"etd":600,"cargo":140},{"imo":"IMO007","eta":360,"etd":660,"cargo":100},{"imo":"IMO008","eta":420,"etd":720,"cargo":150}],"resources":[{"code":"C1","capacity":30,"status":"available"}],"staff":[{"id":"S1","status":"available"}],"storage_areas":[{"code":"Y1","type":"yard","capacity":3000}]}
EOF

    # 3. Large (10 vessels) - EDT expected
    cat > "$FIXTURES_DIR/test-large-10v.json" << 'EOF'
{"requestId":"large-10v","vessels":[{"imo":"IMO001","eta":0,"etd":300,"cargo":100},{"imo":"IMO002","eta":60,"etd":360,"cargo":150},{"imo":"IMO003","eta":120,"etd":420,"cargo":120},{"imo":"IMO004","eta":180,"etd":480,"cargo":180},{"imo":"IMO005","eta":240,"etd":540,"cargo":140},{"imo":"IMO006","eta":300,"etd":600,"cargo":160},{"imo":"IMO007","eta":360,"etd":660,"cargo":130},{"imo":"IMO008","eta":420,"etd":720,"cargo":170},{"imo":"IMO009","eta":480,"etd":780,"cargo":110},{"imo":"IMO010","eta":540,"etd":840,"cargo":190}],"resources":[{"code":"C1","capacity":30,"status":"available"}],"staff":[{"id":"S1","status":"available"}],"storage_areas":[{"code":"Y1","type":"yard","capacity":5000}]}
EOF

    # 4. Multi-crane with delay (should recommend multi)
    cat > "$FIXTURES_DIR/test-multi-with-delay.json" << 'EOF'
{"requestId":"multi-delay","vessels":[{"imo":"IMO001","eta":0,"etd":300,"cargo":150},{"imo":"IMO002","eta":50,"etd":350,"cargo":150}],"resources":[{"code":"C1","capacity":30,"status":"available"},{"code":"C2","capacity":30,"status":"available"}],"staff":[{"id":"S1","status":"available"}],"storage_areas":[{"code":"Y1","type":"yard","capacity":5000}]}
EOF

    # 5. Multi-crane no delay (should recommend single)
    cat > "$FIXTURES_DIR/test-multi-no-delay.json" << 'EOF'
{"requestId":"multi-no-delay","vessels":[{"imo":"IMO001","eta":0,"etd":1000,"cargo":50}],"resources":[{"code":"C1","capacity":30,"status":"available"},{"code":"C2","capacity":30,"status":"available"}],"staff":[{"id":"S1","status":"available"}],"storage_areas":[{"code":"Y1","type":"yard","capacity":5000}]}
EOF

    # 6. Greedy SPT test (varied processing times)
    cat > "$FIXTURES_DIR/test-greedy-spt.json" << 'EOF'
{"requestId":"greedy-spt","vessels":[{"imo":"IMO_LARGE","eta":360,"etd":800,"cargo":300},{"imo":"IMO_SMALL","eta":380,"etd":820,"cargo":50},{"imo":"IMO_MEDIUM","eta":400,"etd":840,"cargo":150}],"resources":[{"code":"C1","capacity":30,"status":"available"}],"staff":[{"id":"S1","status":"available"}],"storage_areas":[{"code":"Y1","type":"yard","capacity":1000}]}
EOF

    echo -e "${GREEN}✓ Generated 6 test fixtures${NC}" | tee -a "$REPORT_FILE"
    echo "" | tee -a "$REPORT_FILE"
}

# ============================================
# FUNCTION: Run Test with Algorithm Validation
# ============================================
run_test_with_validation() {
    local test_name=$1
    local endpoint=$2
    local test_file=$3
    local expected_algorithm=$4
    
    total_tests=$((total_tests + 1))
    
    echo -n "  Running $test_name... " | tee -a "$REPORT_FILE"
    
    local start_time=$(date +%s%N)
    response=$(curl -s -X POST "$API_URL$endpoint" -H "Content-Type: application/json" -d @"$test_file")
    local end_time=$(date +%s%N)
    local duration=$(( (end_time - start_time) / 1000000 ))
    
    echo "$response" | jq '.' > "$RESULTS_DIR/${test_name}_response.json" 2>/dev/null
    
    if ! echo "$response" | jq '.' > /dev/null 2>&1; then
        echo -e "${RED}✗ FAILED (Invalid JSON)${NC}" | tee -a "$REPORT_FILE"
        failed_tests=$((failed_tests + 1))
        echo "$test_name,$endpoint,$expected_algorithm,ERROR,N/A,N/A,false,false" >> "$SUMMARY_FILE"
        return 1
    fi
    
    # Extract response fields
    if [[ "$endpoint" == *"multi-crane"* ]]; then
        success=$(echo "$response" | jq -r '.multiCraneStrategy.success // false')
        delay=$(echo "$response" | jq -r '.multiCraneStrategy.totalDelay // 999999')
        algorithm=$(echo "$response" | jq -r '.multiCraneStrategy.algorithm // "N/A"')
        comp_time=$(echo "$response" | jq -r '.performanceMetrics.computationTimeSeconds // 0')
    else
        success=$(echo "$response" | jq -r '.success // false')
        delay=$(echo "$response" | jq -r '.totalDelay // 999999')
        algorithm=$(echo "$response" | jq -r '.algorithm // "N/A"')
        comp_time=$(echo "$response" | jq -r '.performanceMetrics.computationTimeSeconds // 0')
    fi
    
    # Validate algorithm
    algo_match="true"
    if [ "$expected_algorithm" != "ANY" ] && [ "$algorithm" != "$expected_algorithm" ]; then
        algo_match="false"
    fi
    
    echo "$test_name,$endpoint,$expected_algorithm,$algorithm,$delay,$comp_time,$success,$algo_match" >> "$SUMMARY_FILE"
    
    if [ "$success" = "true" ]; then
        if [ "$algo_match" = "true" ]; then
            echo -e "${GREEN}✓ PASSED${NC} (algo: $algorithm, delay: ${delay}min, time: ${comp_time}s)" | tee -a "$REPORT_FILE"
            passed_tests=$((passed_tests + 1))
        else
            echo -e "${YELLOW}⚠ PASSED with WARNING${NC} (expected: $expected_algorithm, got: $algorithm)" | tee -a "$REPORT_FILE"
            warning_tests=$((warning_tests + 1))
            passed_tests=$((passed_tests + 1))
        fi
        return 0
    else
        error=$(echo "$response" | jq -r '.error // "Unknown"')
        echo -e "${RED}✗ FAILED${NC} ($error)" | tee -a "$REPORT_FILE"
        failed_tests=$((failed_tests + 1))
        return 1
    fi
}

# ============================================
# FUNCTION: Validate Multi-Crane Logic
# ============================================
validate_multi_crane_logic() {
    local test_name=$1
    local test_file=$2
    local expected_recommendation=$3
    
    total_tests=$((total_tests + 1))
    
    echo -n "  Validating $test_name logic... " | tee -a "$REPORT_FILE"
    
    response=$(curl -s -X POST "$API_URL/schedule/multi-crane" -H "Content-Type: application/json" -d @"$test_file")
    
    single_delay=$(echo "$response" | jq -r '.singleCraneStrategy.totalDelay')
    multi_delay=$(echo "$response" | jq -r '.multiCraneStrategy.totalDelay')
    recommendation=$(echo "$response" | jq -r '.comparison.recommendation')
    
    # Validate logic
    local logic_valid="true"
    local error_msg=""
    
    # Rule 1: Single-crane must be tried first
    if [ "$single_delay" = "null" ]; then
        logic_valid="false"
        error_msg="Single-crane strategy not executed"
    fi
    
    # Rule 2: If single delay = 0, should recommend single-crane
    if [ "$single_delay" = "0" ] && [ "$recommendation" != "use_single_crane" ]; then
        logic_valid="false"
        error_msg="Should recommend single-crane when delay=0"
    fi
    
    # Rule 3: If multi-crane recommended, delay should be reduced
    if [ "$recommendation" = "use_multi_crane" ] && [ $(echo "$multi_delay >= $single_delay" | bc) -eq 1 ]; then
        logic_valid="false"
        error_msg="Multi-crane recommended but didn't reduce delay"
    fi
    
    # Rule 4: Check expected recommendation
    if [ "$expected_recommendation" != "ANY" ] && [ "$recommendation" != "$expected_recommendation" ]; then
        logic_valid="false"
        error_msg="Expected $expected_recommendation, got $recommendation"
    fi
    
    if [ "$logic_valid" = "true" ]; then
        echo -e "${GREEN}✓ PASSED${NC} (recommendation: $recommendation)" | tee -a "$REPORT_FILE"
        passed_tests=$((passed_tests + 1))
        return 0
    else
        echo -e "${RED}✗ FAILED${NC} ($error_msg)" | tee -a "$REPORT_FILE"
        failed_tests=$((failed_tests + 1))
        return 1
    fi
}

# ============================================
# GENERATE FIXTURES
# ============================================
generate_fixtures

# ============================================
# TEST SUITE 1: OPTIMAL SCHEDULER
# ============================================
echo -e "${CYAN}========================================${NC}" | tee -a "$REPORT_FILE"
echo -e "${CYAN}Test Suite 1: Optimal Scheduler${NC}" | tee -a "$REPORT_FILE"
echo -e "${CYAN}========================================${NC}" | tee -a "$REPORT_FILE"

run_test_with_validation "optimal-small-3v" "/schedule" "$FIXTURES_DIR/test-small-3v.json" "IARTI_Optimal_BruteForce"
run_test_with_validation "optimal-medium-8v" "/schedule" "$FIXTURES_DIR/test-medium-8v.json" "IARTI_Optimal_BruteForce"
run_test_with_validation "optimal-large-10v" "/schedule" "$FIXTURES_DIR/test-large-10v.json" "IARTI_EDT_Heuristic"

echo "" | tee -a "$REPORT_FILE"

# ============================================
# TEST SUITE 2: GREEDY SCHEDULER
# ============================================
echo -e "${CYAN}========================================${NC}" | tee -a "$REPORT_FILE"
echo -e "${CYAN}Test Suite 2: Greedy SPT Scheduler${NC}" | tee -a "$REPORT_FILE"
echo -e "${CYAN}========================================${NC}" | tee -a "$REPORT_FILE"

run_test_with_validation "greedy-small-3v" "/schedule/greedy" "$FIXTURES_DIR/test-small-3v.json" "IARTI_Greedy_SPT"
run_test_with_validation "greedy-spt-test" "/schedule/greedy" "$FIXTURES_DIR/test-greedy-spt.json" "IARTI_Greedy_SPT"
run_test_with_validation "greedy-large-10v" "/schedule/greedy" "$FIXTURES_DIR/test-large-10v.json" "IARTI_Greedy_SPT"

echo "" | tee -a "$REPORT_FILE"

# ============================================
# TEST SUITE 3: MULTI-CRANE SCHEDULER
# ============================================
echo -e "${CYAN}========================================${NC}" | tee -a "$REPORT_FILE"
echo -e "${CYAN}Test Suite 3: Multi-Crane Optimizer${NC}" | tee -a "$REPORT_FILE"
echo -e "${CYAN}========================================${NC}" | tee -a "$REPORT_FILE"

run_test_with_validation "multi-crane-with-delay" "/schedule/multi-crane" "$FIXTURES_DIR/test-multi-with-delay.json" "IARTI_Multi_Crane_Backtracking"
run_test_with_validation "multi-crane-no-delay" "/schedule/multi-crane" "$FIXTURES_DIR/test-multi-no-delay.json" "IARTI_Multi_Crane_Backtracking"

echo "" | tee -a "$REPORT_FILE"
echo -e "${MAGENTA}Multi-Crane Logic Validation:${NC}" | tee -a "$REPORT_FILE"

validate_multi_crane_logic "multi-logic-with-delay" "$FIXTURES_DIR/test-multi-with-delay.json" "use_multi_crane"
validate_multi_crane_logic "multi-logic-no-delay" "$FIXTURES_DIR/test-multi-no-delay.json" "use_single_crane"

echo "" | tee -a "$REPORT_FILE"

# ============================================
# SUMMARY
# ============================================
echo -e "${YELLOW}========================================${NC}" | tee -a "$REPORT_FILE"
echo -e "${YELLOW}SUMMARY${NC}" | tee -a "$REPORT_FILE"
echo -e "${YELLOW}========================================${NC}" | tee -a "$REPORT_FILE"

echo "" | tee -a "$REPORT_FILE"
echo "Overall Results:" | tee -a "$REPORT_FILE"
echo "  Total tests:  $total_tests" | tee -a "$REPORT_FILE"
echo -e "  ${GREEN}Passed:       $passed_tests${NC}" | tee -a "$REPORT_FILE"
[ $warning_tests -gt 0 ] && echo -e "  ${YELLOW}Warnings:     $warning_tests${NC}" | tee -a "$REPORT_FILE"
[ $failed_tests -gt 0 ] && echo -e "  ${RED}Failed:       $failed_tests${NC}" | tee -a "$REPORT_FILE" || echo "  Failed:       0" | tee -a "$REPORT_FILE"

success_rate=$(awk "BEGIN {printf \"%.1f\", ($passed_tests / $total_tests) * 100}")
echo "  Success rate: $success_rate%" | tee -a "$REPORT_FILE"

echo "" | tee -a "$REPORT_FILE"
echo "Files Generated:" | tee -a "$REPORT_FILE"
echo "  Report:  $REPORT_FILE" | tee -a "$REPORT_FILE"
echo "  CSV:     $SUMMARY_FILE" | tee -a "$REPORT_FILE"
echo "  Results: $RESULTS_DIR/*_response.json" | tee -a "$REPORT_FILE"
echo "" | tee -a "$REPORT_FILE"

# ============================================
# EXIT
# ============================================
if [ $failed_tests -eq 0 ]; then
    if [ $warning_tests -eq 0 ]; then
        echo -e "${GREEN}✓ ALL TESTS PASSED!${NC}" | tee -a "$REPORT_FILE"
    else
        echo -e "${YELLOW}⚠ ALL TESTS PASSED (with $warning_tests warnings)${NC}" | tee -a "$REPORT_FILE"
    fi
    exit 0
else
    echo -e "${RED}✗ SOME TESTS FAILED${NC}" | tee -a "$REPORT_FILE"
    exit 1
fi