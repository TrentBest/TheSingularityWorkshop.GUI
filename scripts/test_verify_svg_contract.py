#!/usr/bin/env python3
"""Focused regression tests for the fail-closed generated-SVG contract."""
from __future__ import annotations

import importlib.util
import tempfile
import unittest
from pathlib import Path

SCRIPT = Path(__file__).with_name("verify_svg_contract.py")
SPEC = importlib.util.spec_from_file_location("verify_svg_contract", SCRIPT)
assert SPEC is not None and SPEC.loader is not None
VALIDATOR = importlib.util.module_from_spec(SPEC)
SPEC.loader.exec_module(VALIDATOR)

SVG = '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 10 10"><title>Test</title><desc>Fixture</desc>{}</svg>'


class SvgContractTests(unittest.TestCase):
    def verify_text(self, source: str) -> list[str]:
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / "fixture.svg"
            path.write_text(source, encoding="utf-8")
            return VALIDATOR.verify(path)

    def test_valid_accessible_svg_passes(self) -> None:
        self.assertEqual([], self.verify_text(SVG.format('<g id="shape"/>')))

    def test_malformed_xml_fails_closed(self) -> None:
        self.assertTrue(any("cannot parse SVG" in e for e in self.verify_text("<svg>")))

    def test_non_svg_root_is_rejected(self) -> None:
        self.assertTrue(any("root element is not SVG" in e for e in self.verify_text(
            '<html><title>Test</title><desc>Fixture</desc></html>')))

    def test_missing_accessibility_text_is_rejected(self) -> None:
        self.assertTrue(any("accessible title" in e for e in self.verify_text(
            '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1 1"><desc>Only</desc></svg>')))

    def test_non_finite_and_non_positive_viewbox_are_rejected(self) -> None:
        for view_box in ("0 0 NaN 10", "0 0 10 Infinity", "0 0 0 10", "0 0 10 -1", "0 0 10"):
            with self.subTest(view_box=view_box):
                source = SVG.replace('viewBox="0 0 10 10"', f'viewBox="{view_box}"').format("")
                self.assertTrue(any("invalid viewBox" in e for e in self.verify_text(source)))

    def test_duplicate_ids_are_rejected(self) -> None:
        errors = self.verify_text(SVG.format('<g id="same"/><g id="same"/>'))
        self.assertTrue(any("duplicate SVG IDs" in e for e in errors))

    def test_unresolved_local_href_is_rejected(self) -> None:
        errors = self.verify_text(SVG.format('<use href="#missing"/>'))
        self.assertTrue(any("unresolved local href" in e for e in errors))

    def test_unresolved_url_reference_is_rejected(self) -> None:
        errors = self.verify_text(SVG.format('<rect style="fill:url(#missing)"/>'))
        self.assertTrue(any("unresolved url(#missing)" in e for e in errors))

    def test_empty_url_reference_is_rejected(self) -> None:
        errors = self.verify_text(SVG.format('<rect style="fill:url(#)"/>'))
        self.assertTrue(any("empty local URL reference" in e for e in errors))

    def test_revision_pinned_repository_source_href_is_allowed(self) -> None:
        revision = "a" * 40
        source = (
            "https://github.com/TrentBest/TheSingularityWorkshop.GUI/"
            f"blob/{revision}/src/Core/DataCard.cs#L12"
        )
        self.assertEqual([], self.verify_text(SVG.format(f'<a href="{source}"><text>Source</text></a>')))

    def test_unpinned_github_source_href_is_rejected(self) -> None:
        source = "https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/development/src/Core/DataCard.cs#L12"
        self.assertTrue(any("non-local href" in e for e in self.verify_text(
            SVG.format(f'<a href="{source}"><text>Source</text></a>'))))

    def test_revision_pinned_github_source_href_is_allowed(self) -> None:
        source = "https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/0123456789abcdef0123456789abcdef01234567/src/Core/DataCard.cs#L12"
        self.assertEqual([], self.verify_text(SVG.format(f'<a href="{source}"><text>DataCard</text></a>')))

    def test_mutable_or_unapproved_external_href_is_rejected(self) -> None:
        for href in (
            "https://example.invalid/image.svg",
            "https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/development/src/Core/DataCard.cs",
            "https://github.com/another/repo/blob/0123456789abcdef0123456789abcdef01234567/src/DataCard.cs",
            "http://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/0123456789abcdef0123456789abcdef01234567/src/DataCard.cs",
        ):
            with self.subTest(href=href):
                self.assertTrue(any("non-local" in e for e in self.verify_text(
                    SVG.format(f'<image href="{href}"/>'))))
        self.assertEqual([], self.verify_text(SVG.format(
            '<image href="data:image/svg+xml;base64,PHN2Zy8+"/>')))

if __name__ == "__main__":
    unittest.main(verbosity=2)
