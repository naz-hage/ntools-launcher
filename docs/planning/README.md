# Internal Technical Planning & Design Documents

> ⚠️ **These are internal technical documents** - not part of the public NuGet package release. They support the roadmap and planning for the YAML Launcher framework implementation.

## Overview

This directory contains internal planning, design decisions, and technical roadmaps for the YAML Launcher framework initiative. These documents are meant to guide implementation and should be reviewed during development phases.

## Document Index

### Architecture & Design

- **[DESIGN-SUMMARY.md](DESIGN-SUMMARY.md)** - Executive architecture overview
  - Problem statement and unified solution approach
  - Layered architecture with 5 phases
  - Backward compatibility strategy
  - Core design decisions and trade-offs
  - Key features and capabilities matrix
  - Risk mitigation plan
  - Estimated 8-10 week delivery timeline

- **[yaml-launcher-design.md](yaml-launcher-design.md)** - Complete architectural specification
  - Business case and problem analysis across three codebases
  - Full YAML schema specification with examples
  - Core API design (8 interfaces)
  - 5-phase implementation strategy with deliverables
  - Success criteria and metrics
  - Recommended: Start here for complete vision

- **[ARCHITECTURAL-DECISIONS.md](ARCHITECTURAL-DECISIONS.md)** - Architecture Decision Records (ADRs)
  - 10 key decisions with rationale and alternatives
  - Tradeoff analysis for each decision
  - Recommended: Review before implementation

### Implementation Planning

- **[implementation-roadmap.md](implementation-roadmap.md)** - Phased implementation plan
  - 5-phase breakdown (Weeks 1-4+)
  - Per-phase deliverables and test files
  - Real-world YAML usage examples
  - Testing strategy and benchmarks
  - Backward compatibility guarantees
  - Documentation plan
  - Recommended: Use as sprint planning guide

### Schema & Reference

- **[yaml-schema-reference.md](yaml-schema-reference.md)** - Complete YAML syntax guide
  - Quick reference for YAML configuration
  - Full schema definition with all properties
  - ExecutableConfig, ExecutionSettings, Variables reference
  - Variable substitution patterns (4 types)
  - 4 production examples (single, sequential, parallel, deployment)
  - Validation rules and best practices
  - Recommended: Reference during YAML development

### Integration & Migration

- **[test-framework-integration.md](test-framework-integration.md)** - test-framework integration strategy ⭐
  - How to extend YAML for test scenarios
  - Backward compatibility guarantee explanation
  - 4-level execution model (simple → assertions → variables)
  - Complete assertion types reference (7 types)
  - Variable extraction patterns with regex examples
  - 5 real-world test scenarios
  - Migration path: test-framework → YAML launcher
  - Recommended: Critical for test-framework team

- **[integration-examples.md](integration-examples.md)** - Real-world scenario examples
  - test-framework multi-step workflow migration
  - nb deployment with verification steps
  - ntools-launcher complex orchestration
  - Parallel testing (multi-framework matrix)
  - Cross-repository coordination example
  - Schema comparison and capability matrix
  - Success metrics and validation approach
  - Recommended: Concrete examples for your use case

### Navigation

- **[DESIGN-PACKAGE-INDEX.md](../DESIGN-PACKAGE-INDEX.md)** - Complete design package index in parent docs directory
  - Searchable index of all design documents
  - Purpose and recommended reading order for each
  - Quick navigation to specific topics

## Reading Guide

### By Role

**Project Manager / Team Lead:**
1. DESIGN-SUMMARY.md - Understand the vision and timeline
2. implementation-roadmap.md - See phase breakdown and deliverables
3. ARCHITECTURAL-DECISIONS.md - Review key decisions and tradeoffs

**Architect / Technical Lead:**
1. yaml-launcher-design.md - Complete architecture
2. ARCHITECTURAL-DECISIONS.md - Design rationale
3. test-framework-integration.md - Integration strategy
4. yaml-schema-reference.md - Schema details

**Development Team:**
1. implementation-roadmap.md - See what you need to build
2. yaml-schema-reference.md - Understand YAML structure
3. integration-examples.md - See usage patterns
4. ARCHITECTURAL-DECISIONS.md - Understand why decisions were made

**Test / Integration Team:**
1. test-framework-integration.md - Your primary document
2. integration-examples.md - See test scenarios
3. yaml-schema-reference.md - Reference for YAML writing

### By Topic

- **Phase planning:** implementation-roadmap.md
- **API design:** yaml-launcher-design.md (Core API section)
- **YAML syntax:** yaml-schema-reference.md
- **Test integration:** test-framework-integration.md
- **Real examples:** integration-examples.md
- **Rationale:** ARCHITECTURAL-DECISIONS.md

## Status

| Document | Status | Last Updated |
|----------|--------|--------------|
| DESIGN-SUMMARY.md | ✅ Complete | May 25, 2026 |
| yaml-launcher-design.md | ✅ Complete | May 25, 2026 |
| ARCHITECTURAL-DECISIONS.md | ✅ Complete | May 25, 2026 |
| implementation-roadmap.md | ✅ Complete | May 25, 2026 |
| yaml-schema-reference.md | ✅ Complete | May 25, 2026 |
| test-framework-integration.md | ✅ Complete | May 25, 2026 |
| integration-examples.md | ✅ Complete | May 25, 2026 |

## Key Milestones

- **Week 1-2:** Phase 1 (Foundation) - Models, YamlLauncherConfigLoader, StepExecutor
- **Week 2:** Phase 2 (Integration) - Sequential orchestration, backward compatibility
- **Week 3:** Phase 3 (Test-Framework) - Assertions, variable extraction, substitution
- **Week 3-4:** Phase 4 (Parallel) - Concurrent execution
- **Week 4+:** Phase 5 (Advanced) - Retries, hooks, filtering

## Next Steps

1. **Review phase:** Stakeholders review DESIGN-SUMMARY.md and ARCHITECTURAL-DECISIONS.md
2. **Planning phase:** Team reviews implementation-roadmap.md and creates sprint backlog
3. **Development phase:** Reference yaml-schema-reference.md, integration-examples.md during implementation
4. **Integration phase:** Use test-framework-integration.md for test-framework collaboration

---

**For questions:** Refer to the specific document sections or review ARCHITECTURAL-DECISIONS.md for rationale on key design choices.

**For updates:** This directory will be updated during implementation to track decisions and adjustments. See git history for evolution of these documents.
