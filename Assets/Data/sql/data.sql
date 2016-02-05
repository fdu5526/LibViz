SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";
-- ----------------------
CREATE TABLE IF NOT EXISTS `InfoTable` (
  `Title` tinytext NOT NULL,
  `Name` tinytext NOT NULL,
  `Time` year(4) NOT NULL,
  `Location` tinytext NOT NULL,
  `Publisher` tinytext NOT NULL,
  `Subtitle` tinytext NOT NULL,
  `Author Translator` tinytext NOT NULL,
  `Note` text NOT NULL,
  `Publish Date` year(4) NOT NULL,
  `Topical Term` tinytext NOT NULL,
  `Form Subdivision` tinytext NOT NULL,
  `General Subdivision` tinytext NOT NULL,
  `Chronological Subdivision` tinytext NOT NULL,
  `Geographic Subdivision` tinytext NOT NULL,
  `Genre` tinytext NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
-- ----------------------
INSERT INTO `InfoTable` (`Title`, `Name`, `Time`, `Location`, `Publisher`, `Subtitle`, `Author Translator`, `Note`, `Publish Date`, `Topical Term`, `Form Subdivision`, `General Subdivision`, `Chronological Subdivision`, `Geographic Subdivision`, `Genre`) VALUES 
('A boat beneath a sunny sky', 'Thomas, Donna', '2012', 'Santa Cruz, CA', 'Peter & Donna Thomas', '-', 'Lewis Caroll ; calligraphy, illustrations, Donna Thomas ; handmade paper, Peter Thomas', 'Title from container; imprint from Colophon', '2012', 'Artists'' books', 'Specimens', '-', '-', '-', '-');
INSERT INTO `InfoTable` (`Title`, `Name`, `Time`, `Location`, `Publisher`, `Subtitle`, `Author Translator`, `Note`, `Publish Date`, `Topical Term`, `Form Subdivision`, `General Subdivision`, `Chronological Subdivision`, `Geographic Subdivision`, `Genre`) VALUES 
('Bon bon mots', 'Chen, Julie', '1998', 'Berkeley, Calif', 'Flying Fish Press', 'a fine assortment of books', 'designed, written, and produced by Julie Chen', 'The box, which is designed to resemble a candy box, contains 3 miniature books (2 of which are accordion folded), 1 folded octagonal object with text, and a small box with text containing 5 copper balls which are to be placed into 5 holes on box''s bottom surface. All are designed to resemble pieces of candy and are nested in a cloth which fits over partitioned bottom of box. A leaf with box''s contents is mounted on the inside lid', '1998', 'Artists'' books', 'Specimens', '-', '-', '-', 'Artists'' books');
INSERT INTO `InfoTable` (`Title`, `Name`, `Time`, `Location`, `Publisher`, `Subtitle`, `Author Translator`, `Note`, `Publish Date`, `Topical Term`, `Form Subdivision`, `General Subdivision`, `Chronological Subdivision`, `Geographic Subdivision`, `Genre`) VALUES 
('Cathedral', 'Munson, Howard', '2004', 'San Francisco', 'Howard Munson', '-', 'Howard Munson', 'Four double-page pop-ups, of purely imagined cathedrals, which can be viewed one at a time in double-paged spreads', '2004', 'Cathedrals', 'Specimens', '-', '-', '-', '-');
INSERT INTO `InfoTable` (`Title`, `Name`, `Time`, `Location`, `Publisher`, `Subtitle`, `Author Translator`, `Note`, `Publish Date`, `Topical Term`, `Form Subdivision`, `General Subdivision`, `Chronological Subdivision`, `Geographic Subdivision`, `Genre`) VALUES 
('Changeling', 'Riker, Maryann J', '2006', 'Phillipsburg, N.J', 'Maryann Riker', '-', 'M. Riker', 'Mixed media artist''s sculpture; one-of-a-kind. A cabinet-shaped structure with two illustrated panels (moon, tape measure, butterfly). These unfold and reveal an intimate interior.  Collage on exterior and interior walls; interior contains mirrors on each panel, plus one shelf with mounted seashell. A third mirror and second seashell stand inside the slotted cabinet', '2006', 'Artists'' books', 'Specimens', '-', '-', '-', 'Artists'' books');
INSERT INTO `InfoTable` (`Title`, `Name`, `Time`, `Location`, `Publisher`, `Subtitle`, `Author Translator`, `Note`, `Publish Date`, `Topical Term`, `Form Subdivision`, `General Subdivision`, `Chronological Subdivision`, `Geographic Subdivision`, `Genre`) VALUES 
('Charles Bukowski papers', 'Bukowski, Charles', '0000', '-', '-', '-', '-', '-', '0000', 'American literature', '-', 'Archival resources', '20th century', 'California', 'Audiotapes');
INSERT INTO `InfoTable` (`Title`, `Name`, `Time`, `Location`, `Publisher`, `Subtitle`, `Author Translator`, `Note`, `Publish Date`, `Topical Term`, `Form Subdivision`, `General Subdivision`, `Chronological Subdivision`, `Geographic Subdivision`, `Genre`) VALUES 
('Gastronomy during wartime', 'Morton, Rebecca Brandeis', '1999', 'San Francisco?', 'Rock Scissors Paper Press', 'a banquet menu', 'illustrated and rendered in carousel form by Rebecca Brandeis Morton ; with a facsimile reprint from L''Almanac gourmand by Charles Monselet', 'Menu drafted by Marechal de Richelieu during the Hanoverian War, printed one course per side on a hexagonal carousel structure, topped by a papier mÃ¢chÃ© covered dish', '1999', 'Artists'' books', 'Miscellanea', '-', '-', 'United States', 'Authors'' autographs (Provenance');
INSERT INTO `InfoTable` (`Title`, `Name`, `Time`, `Location`, `Publisher`, `Subtitle`, `Author Translator`, `Note`, `Publish Date`, `Topical Term`, `Form Subdivision`, `General Subdivision`, `Chronological Subdivision`, `Geographic Subdivision`, `Genre`) VALUES 
('Grell/Colefax Russian Ballet Archive', 'Grell, Dwight', '0000', '-', '-', '-', '-', '-', '0000', 'Ballet', 'Photographs', 'Archival resources', '-', 'Soviet Union', 'Audiotapes');
INSERT INTO `InfoTable` (`Title`, `Name`, `Time`, `Location`, `Publisher`, `Subtitle`, `Author Translator`, `Note`, `Publish Date`, `Topical Term`, `Form Subdivision`, `General Subdivision`, `Chronological Subdivision`, `Geographic Subdivision`, `Genre`) VALUES 
('Hanging laundry', 'Payne, Emily', '1996', 'San Francisco', 'Pea Pod Press', '-', 'Emily Payne', 'Title from cover', '1996', 'Artists'' books', '-', '-', '-', '-', 'Artists'' books (Genre');
INSERT INTO `InfoTable` (`Title`, `Name`, `Time`, `Location`, `Publisher`, `Subtitle`, `Author Translator`, `Note`, `Publish Date`, `Topical Term`, `Form Subdivision`, `General Subdivision`, `Chronological Subdivision`, `Geographic Subdivision`, `Genre`) VALUES 
('I know where I''m going', 'Lingen, Rez''', '1997', 'New York', 'PootÃ© Press', '-', 'compiled by Ruth Lingen & Lois Lane ; with images by Lois Lane', '"Edition of 30 copies plus 6 artist''s proofs ... The images include woodcut, linoleum cut, color copies, photo-engraving, silkscreen and collage"--Colophon', '1997', 'Artists'' books', '-', '-', '-', 'New York', 'Authors'' autographs (Provenance');
INSERT INTO `InfoTable` (`Title`, `Name`, `Time`, `Location`, `Publisher`, `Subtitle`, `Author Translator`, `Note`, `Publish Date`, `Topical Term`, `Form Subdivision`, `General Subdivision`, `Chronological Subdivision`, `Geographic Subdivision`, `Genre`) VALUES 
('Love poems to a vampire', 'Wilde, Denise Carson', '1996', 'Vancouver, B.C., Canada', 'the Author?', '-', 'by D. C. Wilde', 'Poems printed on hardboard planks inserted into wooden coffin, 24 x 11 cm., fastened with metal clasp. Title on metal nameplate on cover', '1996', 'Artists'' books', '-', '-', '20th century', 'Canada', 'Artists'' books (Genre');
INSERT INTO `InfoTable` (`Title`, `Name`, `Time`, `Location`, `Publisher`, `Subtitle`, `Author Translator`, `Note`, `Publish Date`, `Topical Term`, `Form Subdivision`, `General Subdivision`, `Chronological Subdivision`, `Geographic Subdivision`, `Genre`) VALUES 
('The sycamore leaf canopy', 'Cutler-Shaw, Joyce', '2003', 'San Diego', 's.n', '-', '-', 'Edition of 25 copies', '2003', 'Public art', '-', '-', '-', 'California', 'Artists'' books');
INSERT INTO `InfoTable` (`Title`, `Name`, `Time`, `Location`, `Publisher`, `Subtitle`, `Author Translator`, `Note`, `Publish Date`, `Topical Term`, `Form Subdivision`, `General Subdivision`, `Chronological Subdivision`, `Geographic Subdivision`, `Genre`) VALUES 
('World War I ephemera', '-', '0000', '-', '-', '-', '-', '-', '0000', 'Souvenirs (Keepsakes', '-', 'Archival resources', '-', 'Germany', 'Ephemera');
