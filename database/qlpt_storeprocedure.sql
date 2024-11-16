ALTER PROCEDURE GetRoomPosts
    @RoomTypeId INT = NULL
AS
BEGIN
    ;WITH RoomImages AS (
        SELECT 
            ri.post_id,
            ri.image_url,
            ROW_NUMBER() OVER (PARTITION BY ri.post_id ORDER BY ri.image_url) AS RowNum
        FROM Room_Image ri
    )
    SELECT 
        rp.post_id AS PostId,  
        rp.room_name AS RoomName,
        rp.quantity AS Quantity,
        rp.room_description AS RoomDescription,
        ri.image_url AS RoomImage,
        rp.room_price AS RoomPrice,
        rp.room_size AS RoomSize,
        rp.room_coordinate_id AS RoomCoordinateId,
		rp.user_id AS UserId,
        REPLACE(rp.address, 
                SUBSTRING(rp.address, 
                          CHARINDEX(',', rp.address) + 1, 
                          CHARINDEX(',', rp.address, CHARINDEX(',', rp.address) + 1) - CHARINDEX(',', rp.address)), 
                '') AS RoomAddress,
        rt.type_name AS RoomType
    FROM Room_Post rp
    JOIN RoomImages ri ON rp.post_id = ri.post_id AND ri.RowNum = 1
    JOIN Room_Type rt ON rp.room_type_id = rt.room_type_id
    JOIN Room_Status rs ON rp.status_id = rs.room_status_id
    WHERE rs.room_status_id = 1 
      AND (@RoomTypeId IS NULL OR rp.room_type_id = @RoomTypeId);
END



ALTER PROCEDURE GetRoomDetail
    @PostID INT = NULL
AS
BEGIN
    SELECT 
        rp.post_id AS PostId,
        rp.room_name AS RoomName,
        rp.quantity AS Quantity,
        rp.room_description AS RoomDescription,
        rp.room_price AS RoomPrice,
        rp.room_size AS RoomSize,
        rp.address AS RoomAddress,
        rt.type_name AS RoomType, 
        rp.date_posted AS DatePosted,
        rp.expiration_date AS ExpirationDate,
        _user.fullname AS FullName,
        _user.email AS Email,
        _user.gender AS Gender,
        _user.phone AS Phone,
		_user.user_image AS UserImage,
        STRING_AGG(u.utility_name, ', ') AS UtilityNames,
        STRING_AGG(u.description, ', ') AS UtilityDescriptions,
		rc.latitude AS Latitude,
		rc.longitude AS Longitude
    FROM Room_Post rp
    JOIN Room_Type rt ON rp.room_type_id = rt.room_type_id
    JOIN Room_Status rs ON rp.status_id = rs.room_status_id
	JOIN Room_Coordinates rc ON rp.room_coordinate_id = rc.room_coordinate_id
    JOIN [User] _user ON rp.user_id = _user.user_id
    LEFT JOIN Room_Utility ru ON rp.post_id = ru.post_id
    LEFT JOIN Utility u ON ru.utility_id = u.utility_id
    WHERE rs.room_status_id = 1
      AND (@PostID IS NULL OR rp.post_id = @PostID)
    GROUP BY rp.post_id, rp.room_name, rp.quantity, rp.room_description, rp.room_price, rp.room_size, rp.address, rt.type_name, rp.date_posted, rp.expiration_date, _user.fullname, _user.email, _user.gender, _user.phone, _user.user_image, rc.latitude, rc.longitude;

    SELECT 
        ri.image_url AS RoomImage
    FROM Room_Image ri
    WHERE ri.post_id = @PostID
END

ALTER PROCEDURE SearchRoom
    @RoomTypeId INT = NULL, 
    @District NVARCHAR(100) = NULL, 
    @Ward NVARCHAR(100) = NULL, 
    @Adult INT = NULL, 
    @PriceRange NVARCHAR(20) = NULL
AS
BEGIN
    SELECT 
        rp.post_id AS PostId,  
        rp.room_name AS RoomName,
        rp.quantity AS Quantity,
        rp.room_description AS RoomDescription,
        ri.image_url AS RoomImage,
        rp.room_price AS RoomPrice,
        rp.room_size AS RoomSize,
        -- Cắt tên phường, giữ lại tên đường và quận
        REPLACE(rp.address, 
                SUBSTRING(rp.address, 
                          CHARINDEX(',', rp.address) + 1, 
                          CHARINDEX(',', rp.address, CHARINDEX(',', rp.address) + 1) - CHARINDEX(',', rp.address)), 
                '') AS RoomAddress,
        rt.type_name AS RoomType
    FROM Room_Post rp
    JOIN Room_Image ri ON rp.post_id = ri.post_id
    JOIN Room_Type rt ON rp.room_type_id = rt.room_type_id
    JOIN Room_Status rs ON rp.status_id = rs.room_status_id
    WHERE rs.room_status_id = 1  
      AND (@RoomTypeId IS NULL OR rt.room_type_id = @RoomTypeId)  -- Room type filtering
      AND (@Adult IS NULL OR rp.quantity = @Adult)  -- Number of adults filtering
      AND (
            @PriceRange IS NULL OR
            (@PriceRange = 'under-1m' AND rp.room_price < 1000000) OR
            (@PriceRange = '1-2m' AND rp.room_price >= 1000000 AND rp.room_price < 2000000) OR
            (@PriceRange = '2-3m' AND rp.room_price >= 2000000 AND rp.room_price < 3000000) OR
            (@PriceRange = '3-5m' AND rp.room_price >= 3000000 AND rp.room_price < 5000000) OR
            (@PriceRange = '5-7m' AND rp.room_price >= 5000000 AND rp.room_price < 7000000) OR
            (@PriceRange = 'over-7m' AND rp.room_price >= 7000000)
          )  -- Price range filtering
      AND (@District IS NULL OR rp.address LIKE '%' + @District + '%')  -- District filtering
      AND (@Ward IS NULL OR rp.address LIKE '%' + @Ward + '%')  -- Ward filtering
END


select * from Room_Post
select * from Room_Type

EXEC SearchRoom 
    @RoomTypeId = 2, 
    @District = N'Quận 3', 
    @Ward = N'Phường 1', 
    @Adult = 2, 
    @PriceRange = 'over-7m';

select * from Room_Post
EXEC GetRoomDetail @PostID = 1;

SELECT 
    ri.image_url AS RoomImage
FROM Room_Image ri
WHERE ri.post_id = 1
  AND ri.image_type_id IN (1, 6);

EXEC GetRoomPosts @RoomTypeId = 1