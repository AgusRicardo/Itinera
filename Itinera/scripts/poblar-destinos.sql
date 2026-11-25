-- Carga países, ciudades y un destino por ciudad (PostgreSQL).
-- Se puede ejecutar más de una vez: no duplica ni modifica lo que ya existe, y no
-- reactiva lo desactivado. Si algo falla, no se guarda nada.

BEGIN;

CREATE TEMP TABLE carga (pais text NOT NULL, ciudad text NOT NULL) ON COMMIT DROP;

INSERT INTO carga (pais, ciudad)
SELECT datos.pais, unnest(datos.ciudades)
FROM (VALUES
    -- América del Sur
    ('Argentina', ARRAY['Buenos Aires', 'Bariloche', 'Mendoza', 'Salta', 'Córdoba', 'Ushuaia', 'El Calafate', 'Puerto Iguazú', 'Mar del Plata', 'Rosario']),
    ('Brasil', ARRAY['Río de Janeiro', 'San Pablo', 'Florianópolis', 'Salvador de Bahía', 'Búzios', 'Maceió', 'Natal', 'Fortaleza', 'Foz de Iguazú', 'Porto de Galinhas']),
    ('Chile', ARRAY['Santiago', 'Valparaíso', 'Viña del Mar', 'San Pedro de Atacama', 'Puerto Varas', 'Puerto Natales']),
    ('Uruguay', ARRAY['Montevideo', 'Punta del Este', 'Colonia del Sacramento', 'Piriápolis']),
    ('Paraguay', ARRAY['Asunción', 'Encarnación']),
    ('Bolivia', ARRAY['La Paz', 'Uyuni', 'Sucre', 'Copacabana']),
    ('Perú', ARRAY['Lima', 'Cusco', 'Arequipa', 'Puno', 'Paracas', 'Iquitos']),
    ('Colombia', ARRAY['Bogotá', 'Cartagena de Indias', 'Medellín', 'San Andrés', 'Santa Marta', 'Cali']),
    ('Ecuador', ARRAY['Quito', 'Guayaquil', 'Cuenca', 'Puerto Ayora']),
    -- América del Norte, Central y Caribe
    ('Estados Unidos', ARRAY['Nueva York', 'Miami', 'Orlando', 'Los Ángeles', 'Las Vegas', 'San Francisco', 'Chicago', 'Washington D. C.', 'Boston', 'Nueva Orleans', 'San Diego', 'Honolulu']),
    ('Canadá', ARRAY['Toronto', 'Montreal', 'Vancouver', 'Quebec', 'Banff']),
    ('México', ARRAY['Ciudad de México', 'Cancún', 'Playa del Carmen', 'Tulum', 'Guadalajara', 'Oaxaca', 'Puerto Vallarta', 'Los Cabos']),
    ('Cuba', ARRAY['La Habana', 'Varadero', 'Trinidad']),
    ('República Dominicana', ARRAY['Punta Cana', 'Santo Domingo', 'Bayahíbe', 'Samaná']),
    ('Jamaica', ARRAY['Montego Bay', 'Negril']),
    ('Aruba', ARRAY['Oranjestad']),
    ('Curazao', ARRAY['Willemstad']),
    ('Bahamas', ARRAY['Nasáu']),
    ('Costa Rica', ARRAY['San José', 'La Fortuna', 'Tamarindo', 'Manuel Antonio']),
    ('Panamá', ARRAY['Ciudad de Panamá', 'Bocas del Toro']),
    ('Guatemala', ARRAY['Antigua Guatemala', 'Flores']),
    -- Europa
    ('España', ARRAY['Madrid', 'Barcelona', 'Sevilla', 'Valencia', 'Granada', 'Málaga', 'Bilbao', 'San Sebastián', 'Toledo', 'Santiago de Compostela', 'Palma de Mallorca', 'Ibiza']),
    ('Italia', ARRAY['Roma', 'Florencia', 'Venecia', 'Milán', 'Nápoles', 'Verona', 'Bolonia', 'Positano', 'Palermo', 'Pisa', 'Siena', 'Turín']),
    ('Francia', ARRAY['París', 'Niza', 'Lyon', 'Marsella', 'Burdeos', 'Estrasburgo', 'Cannes', 'Aviñón']),
    ('Portugal', ARRAY['Lisboa', 'Oporto', 'Sintra', 'Lagos', 'Funchal']),
    ('Reino Unido', ARRAY['Londres', 'Edimburgo', 'Liverpool', 'Mánchester', 'Bath', 'Oxford']),
    ('Irlanda', ARRAY['Dublín', 'Galway', 'Cork']),
    ('Alemania', ARRAY['Berlín', 'Múnich', 'Hamburgo', 'Fráncfort', 'Colonia', 'Dresde']),
    ('Países Bajos', ARRAY['Ámsterdam', 'Róterdam', 'La Haya', 'Utrecht']),
    ('Bélgica', ARRAY['Bruselas', 'Brujas', 'Gante']),
    ('Suiza', ARRAY['Zúrich', 'Ginebra', 'Lucerna', 'Interlaken', 'Zermatt']),
    ('Austria', ARRAY['Viena', 'Salzburgo', 'Innsbruck', 'Hallstatt']),
    ('República Checa', ARRAY['Praga', 'Český Krumlov']),
    ('Hungría', ARRAY['Budapest']),
    ('Polonia', ARRAY['Cracovia', 'Varsovia']),
    ('Croacia', ARRAY['Dubrovnik', 'Split', 'Zagreb', 'Hvar']),
    ('Grecia', ARRAY['Atenas', 'Santorini', 'Mykonos', 'Heraclión']),
    ('Turquía', ARRAY['Estambul', 'Göreme', 'Antalya', 'Esmirna', 'Pamukkale']),
    ('Dinamarca', ARRAY['Copenhague']),
    ('Suecia', ARRAY['Estocolmo', 'Gotemburgo']),
    ('Noruega', ARRAY['Oslo', 'Bergen', 'Tromsø']),
    ('Finlandia', ARRAY['Helsinki', 'Rovaniemi']),
    ('Islandia', ARRAY['Reikiavik', 'Vík']),
    -- Asia y Medio Oriente
    ('Japón', ARRAY['Tokyo', 'Kioto', 'Osaka', 'Hiroshima', 'Nara', 'Hakone', 'Sapporo']),
    ('China', ARRAY['Pekín', 'Shanghái', 'Xi''an', 'Hong Kong', 'Guilin']),
    ('Corea del Sur', ARRAY['Seúl', 'Busan', 'Jeju']),
    ('Tailandia', ARRAY['Bangkok', 'Chiang Mai', 'Phuket', 'Krabi', 'Ko Samui']),
    ('Vietnam', ARRAY['Hanói', 'Ciudad Ho Chi Minh', 'Hoi An', 'Ha Long']),
    ('Camboya', ARRAY['Siem Reap', 'Nom Pen']),
    ('Indonesia', ARRAY['Ubud', 'Yakarta', 'Yogyakarta', 'Seminyak']),
    ('Singapur', ARRAY['Singapur']),
    ('Malasia', ARRAY['Kuala Lumpur', 'George Town']),
    ('Filipinas', ARRAY['Manila', 'El Nido', 'Cebú']),
    ('India', ARRAY['Nueva Delhi', 'Agra', 'Jaipur', 'Bombay', 'Benarés']),
    ('Nepal', ARRAY['Katmandú', 'Pokhara']),
    ('Maldivas', ARRAY['Malé']),
    ('Emiratos Árabes Unidos', ARRAY['Dubái', 'Abu Dabi']),
    ('Israel', ARRAY['Jerusalén', 'Tel Aviv']),
    ('Jordania', ARRAY['Amán', 'Wadi Musa']),
    -- África
    ('Egipto', ARRAY['El Cairo', 'Luxor', 'Asuán', 'Hurghada']),
    ('Marruecos', ARRAY['Marrakech', 'Fez', 'Chefchaouen', 'Casablanca']),
    ('Sudáfrica', ARRAY['Ciudad del Cabo', 'Johannesburgo', 'Durban']),
    ('Kenia', ARRAY['Nairobi', 'Mombasa']),
    ('Tanzania', ARRAY['Zanzíbar', 'Arusha']),
    -- Oceanía
    ('Australia', ARRAY['Sídney', 'Melbourne', 'Cairns', 'Brisbane', 'Perth']),
    ('Nueva Zelanda', ARRAY['Auckland', 'Queenstown', 'Wellington', 'Rotorua'])
) AS datos (pais, ciudades);

INSERT INTO "Paises" ("Nombre", "Activo")
SELECT DISTINCT carga.pais, true
FROM carga
WHERE NOT EXISTS (
    SELECT 1 FROM "Paises" p WHERE lower(trim(p."Nombre")) = lower(carga.pais));

INSERT INTO "Ciudades" ("PaisId", "Nombre", "Activo")
SELECT pais."Id", carga.ciudad, true
FROM carga
CROSS JOIN LATERAL (
    SELECT p."Id"
    FROM "Paises" p
    WHERE lower(trim(p."Nombre")) = lower(carga.pais) AND p."Activo"
    ORDER BY p."Id"
    LIMIT 1) pais
WHERE NOT EXISTS (
    SELECT 1
    FROM "Ciudades" c
    JOIN "Paises" p ON p."Id" = c."PaisId"
    WHERE lower(trim(p."Nombre")) = lower(carga.pais)
      AND lower(trim(c."Nombre")) = lower(carga.ciudad));

INSERT INTO "Destinos" ("CiudadId", "Activo")
SELECT DISTINCT c."Id", true
FROM "Ciudades" c
JOIN "Paises" p ON p."Id" = c."PaisId"
JOIN carga ON lower(trim(p."Nombre")) = lower(carga.pais)
          AND lower(trim(c."Nombre")) = lower(carga.ciudad)
WHERE c."Activo" AND p."Activo"
  AND NOT EXISTS (SELECT 1 FROM "Destinos" d WHERE d."CiudadId" = c."Id");

COMMIT;

SELECT
    (SELECT count(*) FROM "Paises" WHERE "Activo") AS paises_activos,
    (SELECT count(*) FROM "Ciudades" WHERE "Activo") AS ciudades_activas,
    (SELECT count(*) FROM "Destinos" WHERE "Activo") AS destinos_activos;
