#!/usr/bin/env python3
"""Safety tests for the generated-source-UML promotion boundary."""
from __future__ import annotations
import importlib.util
import tempfile
import unittest
from pathlib import Path

SCRIPT = Path(__file__).with_name("promote_source_uml.py")
SPEC = importlib.util.spec_from_file_location("promote_source_uml", SCRIPT)
assert SPEC is not None and SPEC.loader is not None
PROMOTER = importlib.util.module_from_spec(SPEC)
SPEC.loader.exec_module(PROMOTER)


class SourceUmlPromotionTests(unittest.TestCase):
    def fixture(self, root: Path) -> tuple[Path, Path]:
        source = root / "artifact"
        types = source / "Core.uml-types"
        types.mkdir(parents=True)
        (source / "Core.uml-inventory.md").write_text("# Generated UML type inventory\n", encoding="utf-8")
        (types / "index.md").write_text("# Generated UML type pages\n(TheSingularityWorkshop-Fixture-Type-deadbeef.md)\n(TheSingularityWorkshop-Fixture-Type-deadbeef.svg)\n", encoding="utf-8")
        (types / "TheSingularityWorkshop-Fixture-Type-deadbeef.md").write_text("# Fixture.Type\n\n[diagram](TheSingularityWorkshop-Fixture-Type-deadbeef.svg)\n", encoding="utf-8")
        (types / "TheSingularityWorkshop-Fixture-Type-deadbeef.svg").write_text(
            '<svg xmlns="http://www.w3.org/2000/svg"><title>Fixture.Type</title><desc>Fixture</desc></svg>',
            encoding="utf-8",
        )
        output = root / "docs" / "source-files"
        output.mkdir(parents=True)
        (output / "authored.md").write_text("# Authored file\n", encoding="utf-8")
        return source, output

    def test_promotes_expected_files_and_preserves_authored_files(self) -> None:
        with tempfile.TemporaryDirectory() as temporary:
            source, output = self.fixture(Path(temporary))
            self.assertEqual([], PROMOTER.promote(source, output))
            self.assertTrue((output / "Core.uml-inventory.md").is_file())
            self.assertTrue((output / "Core" / "TheSingularityWorkshop-Fixture-Type-deadbeef.md").is_file())
            self.assertTrue((output / "Core" / "TheSingularityWorkshop-Fixture-Type-deadbeef.svg").is_file())
            self.assertTrue((output / "Core" / "index.md").is_file())
            self.assertEqual("# Authored file\n", (output / "authored.md").read_text(encoding="utf-8"))

    def test_prunes_only_stale_files_named_in_previous_manifest(self) -> None:
        with tempfile.TemporaryDirectory() as temporary:
            source, output = self.fixture(Path(temporary))
            self.assertEqual([], PROMOTER.promote(source, output))
            stale = output / "Core" / "TheSingularityWorkshop-Old-Type-deadbeef.svg"
            stale.write_text("<svg/>", encoding="utf-8")
            manifest = output / PROMOTER.MANIFEST_NAME
            manifest.write_text(manifest.read_text(encoding="utf-8") + "Core/TheSingularityWorkshop-Old-Type-deadbeef.svg\n", encoding="utf-8")
            self.assertEqual([], PROMOTER.promote(source, output))
            self.assertFalse(stale.exists())
            self.assertTrue((output / "authored.md").exists())

    def test_check_mode_detects_drift_without_writing(self) -> None:
        with tempfile.TemporaryDirectory() as temporary:
            source, output = self.fixture(Path(temporary))
            self.assertEqual([], PROMOTER.promote(source, output))
            target = output / "Core" / "TheSingularityWorkshop-Fixture-Type-deadbeef.md"
            target.write_text("# Edited by hand\n", encoding="utf-8")
            errors = PROMOTER.promote(source, output, check=True)
            self.assertTrue(any("stale or missing" in error for error in errors))
            self.assertEqual("# Edited by hand\n", target.read_text(encoding="utf-8"))

    def test_rejects_unsafe_manifest_paths(self) -> None:
        with tempfile.TemporaryDirectory() as temporary:
            source, output = self.fixture(Path(temporary))
            (output / PROMOTER.MANIFEST_NAME).write_text("../authored.md\n", encoding="utf-8")
            with self.assertRaisesRegex(ValueError, "unsafe path"):
                PROMOTER.promote(source, output)

    def test_rejects_unsafe_generated_names(self) -> None:
        with tempfile.TemporaryDirectory() as temporary:
            source, output = self.fixture(Path(temporary))
            (source / "Core.uml-types" / "unexpected.txt").write_text("not UML", encoding="utf-8")
            with self.assertRaisesRegex(ValueError, "non-generated filename"):
                PROMOTER.promote(source, output)

    def test_manifest_cannot_claim_authored_files(self) -> None:
        with tempfile.TemporaryDirectory() as temporary:
            source, output = self.fixture(Path(temporary))
            (output / PROMOTER.MANIFEST_NAME).write_text("Core/README.md\n", encoding="utf-8")
            with self.assertRaisesRegex(ValueError, "not a recognized generated type page/diagram"):
                PROMOTER.promote(source, output)

    def test_rejects_missing_index(self) -> None:
        with tempfile.TemporaryDirectory() as temporary:
            source, output = self.fixture(Path(temporary))
            (source / "Core.uml-types" / "index.md").unlink()
            with self.assertRaisesRegex(ValueError, "missing Core/index.md"):
                PROMOTER.promote(source, output)


if __name__ == "__main__":
    unittest.main(verbosity=2)
