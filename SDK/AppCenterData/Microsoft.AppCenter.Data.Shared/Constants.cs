using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Data
{
    public class Constants
    {
        public static const String TOKEN_RESULT_SUCCEED = "Succeed";

        public static const String DOCUMENT_FIELD_NAME = "document";

        public static const String DOCUMENTS_FIELD_NAME = "Documents";

        public static const String PARTITION_KEY_FIELD_NAME = "PartitionKey";

        public static const String ID_FIELD_NAME = "id";

        public static const String ETAG_FIELD_NAME = "_etag";

        public static const String TIMESTAMP_FIELD_NAME = "_ts";

        static const int PARTITION_KEY_SUFFIX_LENGTH = 37;

        /**
         * Pending operation CREATE value.
         */
        static const String PENDING_OPERATION_CREATE_VALUE = "CREATE";

        /**
         * Pending operation REPLACE value.
         */
        static const String PENDING_OPERATION_REPLACE_VALUE = "REPLACE";

        /**
         * Pending operation DELETE value.
         */
        static const String PENDING_OPERATION_DELETE_VALUE = "DELETE";

        /**
         * Base URL to call token exchange service.
         */
        static const String DEFAULT_API_URL = "https://api.appcenter.ms/v0.1"; //TODO This is not the right url.

        /**
         * Name of the service.
         */
        static const String SERVICE_NAME = "Storage";

        /**
         * Base key for stored preferences.
         */
        private static const String PREFERENCE_PREFIX = SERVICE_NAME + ".";

        /**
         * Cached partition names list file name.
         */
        static const String PREFERENCE_PARTITION_NAMES = PREFERENCE_PREFIX + "partitions";

        /**
         * Cached partition names list file name.
         */
        static const String PREFERENCE_PARTITION_PREFIX = PREFERENCE_PREFIX + "partition.";

        /**
         * TAG used in logging for Storage.
         */
        public static const String LOG_TAG = AppCenterLog.LOG_TAG + SERVICE_NAME;

        /**
         * Constant marking event of the storage group.
         */
        static const String STORAGE_GROUP = "group_storage";

        /**
         * User partition.
         * An authenticated user can read/write documents in this partition.
         */
        public static const String USER = "user";

        /**
         * Readonly partition.
         * Everyone can read documents in this partition.
         * Writes are not allowed via the SDK.
         */
        public static const String READONLY = "readonly";

        /**
         * The continuation token header used to set continuation token.
         */
        public static const String CONTINUATION_TOKEN_HEADER = "x-ms-continuation";
    }
}