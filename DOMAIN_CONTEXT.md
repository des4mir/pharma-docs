# PharmaDocs — Domain Context

This document explains the pharmaceutical regulatory domain that PharmaDocs models.  
It is intended for developers who are unfamiliar with Health Canada processes.

---

## What Is Health Canada?

Health Canada is the federal department responsible for regulating pharmaceutical products sold in Canada. Before a drug can be sold, the manufacturer must submit a regulatory dossier proving the product is safe, effective, and of acceptable quality.

---

## Submission Types

### NDS — New Drug Submission

Filed when a pharmaceutical company wants to sell a brand-new drug in Canada for the first time. Requires the most comprehensive documentation — full clinical trial data, manufacturing quality evidence, and proposed labelling (Product Monograph).

### ANDS — Abbreviated New Drug Submission

Filed for generic drugs. Instead of full clinical trials, the company demonstrates that their product is bioequivalent to an already-approved reference drug. Significantly less documentation than an NDS.

### SNDS — Supplement to New Drug Submission

Filed when a company wants to change something about an already-approved drug — a new indication, a new dosage form, a new strength, or a manufacturing change. Think of it as an amendment to an existing approval.

### DMF — Drug Master File

A confidential dossier submitted by a manufacturer (often an API supplier) that contains detailed information about a facility, process, or component. Referenced by other submissions without disclosing proprietary details to the applicant.

### CTA — Clinical Trial Application

Filed before a clinical trial can begin in Canada. Health Canada reviews the protocol and safety data to authorize human testing.

---

## Document Types in PharmaDocs

| Document Type               | Health Canada Module      | Description                                                 |
| --------------------------- | ------------------------- | ----------------------------------------------------------- |
| Certificate of Analysis     | Module 3 (Quality)        | Lab test results confirming a batch meets specifications    |
| Product Monograph           | Module 1 (Administrative) | Official Canadian product labelling document                |
| Batch Record                | Module 3 (Quality)        | Manufacturing record for a specific production batch        |
| Temperature & Storage Log   | GDP/GSP Compliance        | Cold chain and storage condition records                    |
| Product Specification Sheet | Module 3 (Quality)        | Defined quality standards for the product                   |
| Submission Certificate      | Module 1 (Administrative) | Signed cover letter from senior executive + medical officer |
| Clinical Study Report       | Module 5 (Clinical)       | Full report of a completed clinical trial                   |
| Non-Clinical Study Report   | Module 4 (Non-Clinical)   | Animal and in-vitro study results                           |
| Quality Summary             | Module 3 (Quality)        | Executive summary of the quality dossier                    |
| Import Clearance            | Module 1 (Administrative) | Authorization to import product into Canada                 |

---

## eCTD — Electronic Common Technical Document

eCTD is the internationally standardized format Health Canada requires for drug submissions. It organizes documents into 5 modules:

- **Module 1** — Administrative (Canada-specific): cover letters, application forms, Product Monograph
- **Module 2** — Summaries: quality, non-clinical, and clinical overviews
- **Module 3** — Quality: chemistry, manufacturing, and controls
- **Module 4** — Non-Clinical: pharmacology and toxicology studies
- **Module 5** — Clinical: human trial data

PharmaDocs does not generate eCTD XML. It models the **document tracking and workflow layer** that precedes and supports eCTD filing.

---

## Key Identifiers

**DIN — Drug Identification Number**  
Assigned by Health Canada to every approved drug product sold in Canada. A product does not have a DIN until it receives market authorization. In PharmaDocs, DIN is nullable because it may not exist yet at the time of submission.

**NPN — Natural Product Number**  
Equivalent to DIN but for natural health products (vitamins, herbal remedies, homeopathic medicines) regulated under the Natural Health Products Regulations. Also nullable for the same reason.

---

## Workflow Summary

```
Product Created
↓
Document Records assembled (CoA, Batch Record, Product Monograph, etc.)
↓
Submission Package created (NDS / ANDS / SNDS / DMF / CTA)
↓
Documents linked to Submission Package
↓
Package status: Draft → Submitted → Under Review → Approved / Rejected / Withdrawn
↓
Every status change recorded in AuditLog (who changed it, when, and why)
```

---

## Regulatory Affairs Officer vs. Viewer

**RegAffairsOfficer** — Can create products, upload documents, create submission packages, and change submission status.  
**Viewer** — Read-only access. Suitable for quality managers, executives, or external auditors who need visibility without write access.
